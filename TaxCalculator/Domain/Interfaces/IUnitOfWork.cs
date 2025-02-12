namespace ShaamCoreTemplate.Domain.Interfaces;

public interface IUnitOfWork
{
    Lazy<ISampleRepository> SampleRepository { get; }
    Lazy<IUserRepository> UserRepository { get; }
}
