namespace ContentShare.API.Middlewares;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class UseHmacAttribute : Attribute
{
}
