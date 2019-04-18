
namespace WebSocket.Events
{
    public delegate void Events<in T>(T t);
    public delegate void Events<in T, in T2>(T t, T2 t2);
    public delegate void Events<in T, in T2, in T3>(T t, T2 t2, T3 t3);
}
