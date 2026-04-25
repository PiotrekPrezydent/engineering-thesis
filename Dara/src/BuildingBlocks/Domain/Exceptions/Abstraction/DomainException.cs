namespace Dara.BuildingBlocks.Domain.Exceptions.Abstraction
{
    public abstract class DomainException : Exception
    {
        public string ModuleName => TargetSite!.Module.Assembly.GetName().Name!;

        protected string _wall => new string(':', 128);
    
        protected abstract string DomainExcetpionState();

        public DomainException(string message) : base(message)
        {
        
        }

        public override string Message => GetBuildedMessage();

        public void PrintBuildedMessage()
        {
            Console.WriteLine(GetBuildedMessage());
        }
    
        public string GetBuildedMessage()
        {
            string ret = "\n\n\n";
        
            ret += _wall + "\n\n";
            ret += $"Readed Domain Exception\n\n";
        
            ret += $"MODULE:\t[ {ModuleName} ]\n";
            ret += $"TYPE:\t[ {GetType()} ]\n\n";
        
            ret += $"{Message}\n\n";
        
            ret += DomainExcetpionState() + "\n";
        
            ret += $"{_wall}\n\n";
        
        
            return ret;
        }
    }
}