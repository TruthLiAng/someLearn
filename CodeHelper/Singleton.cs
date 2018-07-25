//基于双重检查锁定的正确单例模式实现
public class Singleton {
    private static Singleton instance=null;//加入volatile修饰
    public static Singleton getInstance() {
        if(null==instance){
            synchronized (Singleton.class){
                if(null==instance){
                    instance = new Singleton();
                }
            }
        }
        return instance;
    }
}
