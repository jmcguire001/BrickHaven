namespace BrickHaven.Models
{
    public interface ILegoRepository
    {
        // Query the instances from Lego model and save to set Lego
        public IQueryable<Lego> Legos { get; }
    }
}
