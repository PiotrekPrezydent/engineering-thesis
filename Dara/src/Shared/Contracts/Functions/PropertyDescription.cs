namespace Dara.Shared.Contracts.Functions;

// a function parameter or an object property, required ones go to the "required" list
public record PropertyDescription(string Name, TypeDescription Type, bool Required);
