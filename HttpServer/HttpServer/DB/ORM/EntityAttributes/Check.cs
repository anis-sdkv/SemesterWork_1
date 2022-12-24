namespace HttpServer.DB.ORM.EntityAttributes;

/// <summary>
/// Use * instead of field name
/// Example CHECK(* > 0 AND * < 100) or CHECK (role IN('r','a', 'm'))
/// </summary>
public class Check : Attribute
{
    public readonly string Condition;
    public Check(string condition)
    {
        Condition = condition;
    }
}