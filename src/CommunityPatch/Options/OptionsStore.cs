namespace CommunityPatch {

  public abstract class OptionsStore {

    public abstract void Save();

    public abstract void Set<T>(string key, T value);

    public abstract void Set<T>(string ns, string key, T value);

    public abstract T Get<T>(string key);

    public abstract T Get<T>(string ns, string key);

  }

}