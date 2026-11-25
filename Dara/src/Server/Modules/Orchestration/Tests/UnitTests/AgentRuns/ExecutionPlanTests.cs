using Dara.Server.Modules.Orchestration.Domain.AgentRuns.ExecutionPlans;
using Dara.Server.Modules.Orchestration.Domain.AgentRuns.Rules;
using static Dara.Server.Modules.Orchestration.UnitTests.OrchestrationSamples;

namespace Dara.Server.Modules.Orchestration.UnitTests.AgentRuns;

// calls proposed through the run: alias, arguments against the description, results of earlier calls
public class ExecutionPlanTests
{
    [Fact]
    public void CallsAreNumberedInTheOrderOfTheModel()
    {
        var run = StartRun(out var phone, out _);

        var first = run.Propose("BobsPhone_GetBatteryLevel");
        var second = run.Propose("BobsPhone_SaveFile", """{"path":"/battery.txt","content":"full"}""");
        var third = run.Propose("BobsPhone_Ping");

        Assert.Equal(new[] { 1, 2, 3 }, new[] { first, second, third });
        Assert.Equal(new[] { 1, 2, 3 }, run.Plan.Calls.Select(c => c.Index));

        var saveFile = run.Plan.Calls[1];
        Assert.Equal("BobsPhone_SaveFile", saveFile.Alias);
        Assert.Equal(phone.ParticipantId, saveFile.ParticipantId.Value);
        Assert.Equal(phone.Functions.Single(f => f.Description.Name == "SaveFile").FunctionId, saveFile.FunctionId.Value);
        Assert.Equal(new[] { "path", "content" }, saveFile.Arguments.Select(a => a.Name));
        Assert.All(saveFile.Arguments, a => Assert.Equal(CallArgumentKind.Value, a.Kind));
        Assert.Equal("\"/battery.txt\"", saveFile.Arguments[0].Json);
    }

    [Fact]
    public void AliasWithOtherLetterCaseIsStoredAsGiven()
    {
        var run = StartRun();

        run.Propose("bobsphone_ping");

        Assert.Equal("BobsPhone_Ping", run.Plan.Calls[0].Alias);
    }

    [Fact]
    public void UnknownAliasIsRejected()
    {
        var exception = AssertBroken<ProposedCallMustUseKnownToolRule>(() => StartRun().Propose("BobsPhone_Notify"));

        Assert.Contains("BobsPhone_Notify", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not json")]
    [InlineData("[1,2]")]
    [InlineData("\"path\"")]
    public void ArgumentsMustBeAJsonObject(string argumentsJson)
    {
        AssertBroken<ProposedArgumentsMustBeJsonObjectRule>(() => StartRun().Propose("BobsPhone_Ping", argumentsJson));
    }

    [Theory]
    [InlineData("""{"path":"/a","content":"x","colour":"red"}""", "unknown argument colour")]
    [InlineData("""{"path":"/a"}""", "required argument content is missing")]
    [InlineData("""{"path":"/a","path":"/b","content":"x"}""", "argument path is given more than once")]
    [InlineData("""{"path":"a.txt","content":"x"}""", "path: does not match the pattern ^/")]
    [InlineData("""{"path":"/a","content":5}""", "content: expected a string")]
    [InlineData("""{"path":"/a","content":null}""", "content: null is not allowed")]
    [InlineData("""{"path":"/a","content":"x","overwrite":"yes"}""", "overwrite: expected a boolean")]
    public void WrongArgumentsSayWhatToFix(string argumentsJson, string problem)
    {
        var exception = AssertBroken<CallArgumentsMustMatchFunctionRule>(() => StartRun().Propose("BobsPhone_SaveFile", argumentsJson));

        Assert.Contains(problem, exception.Message);
    }

    [Theory]
    [InlineData("""{"message":"hi","level":0}""", "level: expected at least 1")]
    [InlineData("""{"message":"hi","level":1.5}""", "level: expected an integer")]
    public void NumbersKeepTheirLimits(string argumentsJson, string problem)
    {
        var exception = AssertBroken<CallArgumentsMustMatchFunctionRule>(() => StartRun().Propose("AliceLaptop_Notify", argumentsJson));

        Assert.Contains(problem, exception.Message);
    }

    [Fact]
    public void OptionalArgumentsMayBeLeftOut()
    {
        var run = StartRun();

        run.Propose("AliceLaptop_Notify", """{"message":"hi"}""");
        run.Propose("AliceLaptop_Notify", """{"message":"hi","level":2.0}""");

        Assert.Equal(2, run.Plan.Calls.Count);
    }

    [Fact]
    public void WholeResultOfAnEarlierCallIsAReference()
    {
        var run = StartRun();
        run.Propose("BobsPhone_ReadFile", """{"path":"/notes.txt"}""");

        run.Propose("BobsPhone_SaveFile", """{"path":"/copy.txt","content":" {{result:1}} "}""");

        var content = run.Plan.Calls[1].Arguments.Single(a => a.Name == "content");
        Assert.Equal(CallArgumentKind.Result, content.Kind);
        Assert.Equal(new[] { 1 }, content.ResultIndexes);
    }

    [Theory]
    [InlineData("{{result:2}}")]
    [InlineData("{{result:3}}")]
    [InlineData("{{result:0}}")]
    [InlineData("{{result:99999999999}}")]
    public void ReferenceMustPointToAnEarlierCall(string reference)
    {
        var run = StartRun();
        run.Propose("BobsPhone_ReadFile", """{"path":"/notes.txt"}""");

        AssertBroken<ResultReferenceMustPointToEarlierCallRule>(() =>
            run.Propose("BobsPhone_SaveFile", $$"""{"path":"/copy.txt","content":"{{reference}}"}"""));
        Assert.Single(run.Plan.Calls);
    }

    [Fact]
    public void FirstCallCannotUseResults()
    {
        var exception = AssertBroken<ResultReferenceMustPointToEarlierCallRule>(() =>
            StartRun().Propose("BobsPhone_ReadFile", """{"path":"{{result:1}}"}"""));

        Assert.Contains("Call 1 is the first call", exception.Message);
    }

    [Fact]
    public void ResultMustFitTheParameter()
    {
        var run = StartRun();
        run.Propose("BobsPhone_GetBatteryLevel");
        run.Propose("BobsPhone_Ping");
        run.Propose("AliceLaptop_ListFiles", """{"folder":"/"}""");

        var exception = AssertBroken<ResultTypeMustMatchParameterRule>(() =>
            run.Propose("BobsPhone_ReadFile", """{"path":"{{result:1}}"}"""));
        Assert.Contains("call 1 returns integer, the argument needs string", exception.Message);

        AssertBroken<ResultTypeMustMatchParameterRule>(() =>
            run.Propose("BobsPhone_ReadFile", """{"path":"{{result:2}}"}"""));
        AssertBroken<ResultTypeMustMatchParameterRule>(() =>
            run.Propose("BobsPhone_ReadFile", """{"path":"{{result:3}}"}"""));
    }

    [Fact]
    public void FittingResultsArePassedOn()
    {
        var run = StartRun();
        run.Propose("BobsPhone_GetBatteryLevel");
        run.Propose("AliceLaptop_ListFiles", """{"folder":"/"}""");

        // an integer fits a number, a list of strings fits a list of strings
        run.Propose("AliceLaptop_SetVolume", """{"level":"{{result:1}}"}""");
        run.Propose("AliceLaptop_SendFiles", """{"paths":"{{result:2}}"}""");

        Assert.Equal(4, run.Plan.Calls.Count);
    }

    [Fact]
    public void ResultsInsideATextArgumentAreKept()
    {
        var run = StartRun();
        run.Propose("BobsPhone_GetBatteryLevel");
        run.Propose("AliceLaptop_ListFiles", """{"folder":"/"}""");

        run.Propose("AliceLaptop_Notify", """{"message":"Battery {{result:1}}%, files {{result:2}}, {{result:1}}"}""");

        var message = run.Plan.Calls[2].Arguments.Single();
        Assert.Equal(CallArgumentKind.Text, message.Kind);
        Assert.Equal(new[] { 1, 2 }, message.ResultIndexes);
    }

    [Fact]
    public void TextWithResultsFitsTextArgumentsOnly()
    {
        var run = StartRun();
        run.Propose("BobsPhone_GetBatteryLevel");

        AssertBroken<CallArgumentsMustMatchFunctionRule>(() =>
            run.Propose("AliceLaptop_SetVolume", """{"level":"level {{result:1}}"}"""));
    }

    [Fact]
    public void VoidResultCannotGoIntoText()
    {
        var run = StartRun();
        run.Propose("BobsPhone_Ping");

        var exception = AssertBroken<ResultTypeMustMatchParameterRule>(() =>
            run.Propose("AliceLaptop_Notify", """{"message":"ping said {{result:1}}"}"""));
        Assert.Contains("call 1 returns nothing", exception.Message);
    }

    [Fact]
    public void ResultInsideAnArrayIsNotSupported()
    {
        var run = StartRun();
        run.Propose("BobsPhone_ReadFile", """{"path":"/a"}""");

        var exception = AssertBroken<CallArgumentsMustMatchFunctionRule>(() =>
            run.Propose("AliceLaptop_SendFiles", """{"paths":["/b","{{result:1}}"]}"""));
        Assert.Contains("not a part of an object or an array", exception.Message);
    }
}
