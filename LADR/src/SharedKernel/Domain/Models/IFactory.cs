namespace LADR.SharedKernel.Domain.Models;

public interface IFactory <in TEntity> where TEntity : Entity
{
}