namespace SafeDadta
{
    public interface ISafe
    {
        public void SafeCurrentId(int levelNumber);
        public int LoadCurrentId();
    }
}