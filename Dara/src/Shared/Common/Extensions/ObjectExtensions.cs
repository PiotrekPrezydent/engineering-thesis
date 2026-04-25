namespace Dara.Shared.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static IEnumerable<string> GetObjectState(this object obj)
        {
            List<string> ret = new();
        
            Type type = obj.GetType();
            ret.Add(type.Name);
        
            foreach (var propertyInfo in type.GetProperties())
            {
                
            }

            return ret;
        }
    }
}