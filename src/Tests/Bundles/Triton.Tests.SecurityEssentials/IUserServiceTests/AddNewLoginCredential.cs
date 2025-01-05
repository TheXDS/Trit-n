#pragma warning disable CS1591

using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using System.Security;
using TheXDS.MCART.Security;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Tests.SecurityEssentials.IUserServiceTests;

public class AddNewLoginCredential
{
    [Test]
    public async Task AddNewLoginCredential_adds_new_credential()
    {
        var transactionMock = new Mock<ICrudReadWriteTransaction>();
        var svcMock = new Mock<IUserService>() { CallBase = true };
        UserGroup[] testGroups = [new(), new(), new()];
        LoginCredential? newCredential = null;

        transactionMock.Setup(p => p.SearchAsync(It.IsAny<Expression<Func<LoginCredential, bool>>>()))
            .ReturnsAsync(new ServiceResult<LoginCredential[]?>([]))
            .Verifiable(Times.Once);
        transactionMock.Setup(p => p.ReadAsync<LoginCredential, Guid>(It.IsAny<Guid>()))
            .ReturnsAsync(new ServiceResult<LoginCredential?>((LoginCredential?)null))
            .Verifiable(Times.AtLeastOnce);
        transactionMock.Setup(p => p.Create(It.IsAny<LoginCredential>()))
            .Callback<LoginCredential[]>(p => newCredential = p[0])
            .Returns(ServiceResult.Ok)
            .Verifiable(Times.Once);
        transactionMock.Setup(p => p.CommitAsync())
            .ReturnsAsync(ServiceResult.Ok)
            .Verifiable(Times.Once);

        svcMock.Setup(p => p.GetTransaction())
            .Returns(transactionMock.Object)
            .Verifiable(Times.Once);
        svcMock.Setup(p => p.HashPasswordAsync<Pbkdf2Storage>(It.IsAny<SecureString>()))
            .ReturnsAsync([1, 2, 3, 4])
            .Verifiable(Times.Once);

        var result = await svcMock.Object.AddNewLoginCredential<Pbkdf2Storage>("test", "password".ToSecureString(), PermissionFlags.Special, PermissionFlags.Special, true, true, testGroups);

        Assert.That(result, Is.EqualTo(ServiceResult.Ok));        
        Assert.Multiple(() =>
        {
            Assert.That(newCredential, Is.Not.Null);
            Assert.That(newCredential?.Id, Is.Not.EqualTo(default(Guid)));
            Assert.That(newCredential?.Username, Is.EqualTo("test"));
            Assert.That(newCredential?.PasswordHash, Is.EquivalentTo((byte[])[1, 2, 3, 4]));
            Assert.That(newCredential?.PasswordChangeScheduled, Is.True);
            Assert.That(newCredential?.Granted, Is.EqualTo(PermissionFlags.Special));
            Assert.That(newCredential?.Revoked, Is.EqualTo(PermissionFlags.Special));
            Assert.That(newCredential?.Enabled, Is.True);
            Assert.That(newCredential?.Membership.Count, Is.EqualTo(3));
            Assert.That(newCredential?.Membership.ToArray()[0].Group, Is.SameAs(testGroups[0]));
            Assert.That(newCredential?.Membership.ToArray()[0].SecurityObject, Is.SameAs(newCredential));
            Assert.That(newCredential?.Membership.ToArray()[1].Group, Is.SameAs(testGroups[1]));
            Assert.That(newCredential?.Membership.ToArray()[1].SecurityObject, Is.SameAs(newCredential));
            Assert.That(newCredential?.Membership.ToArray()[2].Group, Is.SameAs(testGroups[2]));
            Assert.That(newCredential?.Membership.ToArray()[2].SecurityObject, Is.SameAs(newCredential));
            Assert.That(newCredential?.Sessions, Is.Empty);
            Assert.That(newCredential?.RegisteredMfa, Is.Empty);
            Assert.That(newCredential?.Descriptors, Is.Empty);
        });
        transactionMock.Verify();
        svcMock.Verify();
    }
    
    [Test]
    public async Task AddNewLoginCredential_with_default_params_adds_new_credential()
    {
        var transactionMock = new Mock<ICrudReadWriteTransaction>();
        var svcMock = new Mock<IUserService>() { CallBase = true };
        LoginCredential? newCredential = null;

        transactionMock.Setup(p => p.SearchAsync(It.IsAny<Expression<Func<LoginCredential, bool>>>()))
            .ReturnsAsync(new ServiceResult<LoginCredential[]?>([]))
            .Verifiable(Times.Once);
        transactionMock.Setup(p => p.ReadAsync<LoginCredential, Guid>(It.IsAny<Guid>()))
            .ReturnsAsync(new ServiceResult<LoginCredential?>((LoginCredential?)null))
            .Verifiable(Times.AtLeastOnce);
        transactionMock.Setup(p => p.Create(It.IsAny<LoginCredential>()))
            .Callback<LoginCredential[]>(p => newCredential = p[0])
            .Returns(ServiceResult.Ok)
            .Verifiable(Times.Once);
        transactionMock.Setup(p => p.CommitAsync())
            .ReturnsAsync(ServiceResult.Ok)
            .Verifiable(Times.Once);

        svcMock.Setup(p => p.GetTransaction())
            .Returns(transactionMock.Object)
            .Verifiable(Times.Once);
        svcMock.Setup(p => p.HashPasswordAsync<Pbkdf2Storage>(It.IsAny<SecureString>()))
            .ReturnsAsync([1, 2, 3, 4])
            .Verifiable(Times.Once);

        var result = await svcMock.Object.AddNewLoginCredential<Pbkdf2Storage>("test", "password".ToSecureString());

        Assert.That(result, Is.EqualTo(ServiceResult.Ok));
        Assert.Multiple(() =>
        {
            Assert.That(newCredential, Is.Not.Null);
            Assert.That(newCredential?.Id, Is.Not.EqualTo(default(Guid)));
            Assert.That(newCredential?.Username, Is.EqualTo("test"));
            Assert.That(newCredential?.PasswordHash, Is.EquivalentTo((byte[])[1, 2, 3, 4]));
            Assert.That(newCredential?.PasswordChangeScheduled, Is.False);
            Assert.That(newCredential?.Granted, Is.EqualTo(PermissionFlags.None));
            Assert.That(newCredential?.Revoked, Is.EqualTo(PermissionFlags.None));
            Assert.That(newCredential?.Enabled, Is.True);
            Assert.That(newCredential?.Membership, Is.Empty);
            Assert.That(newCredential?.Sessions, Is.Empty);
            Assert.That(newCredential?.RegisteredMfa, Is.Empty);
            Assert.That(newCredential?.Descriptors, Is.Empty);
        });
        transactionMock.Verify();
        svcMock.Verify();
    }

    [Test]
    public async Task AddNewLoginCredential_with_default_params_and_no_TAlg_defaults_to_pbkdf2()
    {
        var transactionMock = new Mock<ICrudReadWriteTransaction>();
        var svcMock = new Mock<IUserService>() { CallBase = true };
        LoginCredential? newCredential = null;

        transactionMock.Setup(p => p.SearchAsync(It.IsAny<Expression<Func<LoginCredential, bool>>>()))
            .ReturnsAsync(new ServiceResult<LoginCredential[]?>([]))
            .Verifiable(Times.Once);
        transactionMock.Setup(p => p.ReadAsync<LoginCredential, Guid>(It.IsAny<Guid>()))
            .ReturnsAsync(new ServiceResult<LoginCredential?>((LoginCredential?)null))
            .Verifiable(Times.AtLeastOnce);
        transactionMock.Setup(p => p.Create(It.IsAny<LoginCredential>()))
            .Callback<LoginCredential[]>(p => newCredential = p[0])
            .Returns(ServiceResult.Ok)
            .Verifiable(Times.Once);
        transactionMock.Setup(p => p.CommitAsync())
            .ReturnsAsync(ServiceResult.Ok)
            .Verifiable(Times.Once);

        svcMock.Setup(p => p.GetTransaction())
            .Returns(transactionMock.Object)
            .Verifiable(Times.Once);
        svcMock.Setup(p => p.HashPasswordAsync<Pbkdf2Storage>(It.IsAny<SecureString>()))
            .ReturnsAsync([1, 2, 3, 4])
            .Verifiable(Times.Once);

        var result = await svcMock.Object.AddNewLoginCredential("test", "password".ToSecureString());

        Assert.That(result, Is.EqualTo(ServiceResult.Ok));
        Assert.Multiple(() =>
        {
            Assert.That(newCredential, Is.Not.Null);
            Assert.That(newCredential?.Id, Is.Not.EqualTo(default(Guid)));
            Assert.That(newCredential?.Username, Is.EqualTo("test"));
            Assert.That(newCredential?.PasswordHash, Is.EquivalentTo((byte[])[1, 2, 3, 4]));
            Assert.That(newCredential?.PasswordChangeScheduled, Is.False);
            Assert.That(newCredential?.Granted, Is.EqualTo(PermissionFlags.None));
            Assert.That(newCredential?.Revoked, Is.EqualTo(PermissionFlags.None));
            Assert.That(newCredential?.Enabled, Is.True);
            Assert.That(newCredential?.Membership, Is.Empty);
            Assert.That(newCredential?.Sessions, Is.Empty);
            Assert.That(newCredential?.RegisteredMfa, Is.Empty);
            Assert.That(newCredential?.Descriptors, Is.Empty);
        });
        transactionMock.Verify();
        svcMock.Verify();
    }

    [Test]
    public async Task AddNewLoginCredential_user_dup_ckech_fail_returns_failure()
    {
        var transactionMock = new Mock<ICrudReadWriteTransaction>();
        var svcMock = new Mock<IUserService>() { CallBase = true };

        transactionMock.Setup(p => p.SearchAsync(It.IsAny<Expression<Func<LoginCredential, bool>>>()))
            .ReturnsAsync(new ServiceResult<LoginCredential[]?>(FailureReason.NetworkFailure))
            .Verifiable(Times.Once);

        svcMock.Setup(p => p.GetTransaction())
            .Returns(transactionMock.Object)
            .Verifiable(Times.Once);

        var result = await svcMock.Object.AddNewLoginCredential("test", "password".ToSecureString());

        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.NetworkFailure));
        transactionMock.Verify();
        svcMock.Verify();
    }

    [Test]
    public async Task AddNewLoginCredential_user_duplication_returns_EntityDuplication()
    {
        var transactionMock = new Mock<ICrudReadWriteTransaction>();
        var svcMock = new Mock<IUserService>() { CallBase = true };

        transactionMock.Setup(p => p.SearchAsync(It.IsAny<Expression<Func<LoginCredential, bool>>>()))
            .ReturnsAsync(new ServiceResult<LoginCredential[]?>([ new() ]))
            .Verifiable(Times.Once);

        svcMock.Setup(p => p.GetTransaction())
            .Returns(transactionMock.Object)
            .Verifiable(Times.Once);

        var result = await svcMock.Object.AddNewLoginCredential("test", "password".ToSecureString());

        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.EntityDuplication));
        transactionMock.Verify();
        svcMock.Verify();
    }

    [Test]
    public async Task AddNewLoginCredential_with_id_collision_changes_id_until_resolved()
    {
        var transactionMock = new Mock<ICrudReadWriteTransaction>();
        var svcMock = new Mock<IUserService>() { CallBase = true };
        HashSet<Guid> differentGuids = [];
        LoginCredential?[] duplicationCheckFakeResults = [
            new(),
            new(),
            new(),
            null
        ];
        int dupCheckCalls = 0;
        ServiceResult<LoginCredential?> GetDuplicationCheckFakeResult() => duplicationCheckFakeResults[dupCheckCalls++];

        transactionMock.Setup(p => p.SearchAsync(It.IsAny<Expression<Func<LoginCredential, bool>>>()))
            .ReturnsAsync(new ServiceResult<LoginCredential[]?>([]))
            .Verifiable(Times.Once);
        transactionMock.Setup(p => p.ReadAsync<LoginCredential, Guid>(It.IsAny<Guid>()))
            .Callback<Guid>(p => differentGuids.Add(p))
            .ReturnsAsync(GetDuplicationCheckFakeResult)
            .Verifiable(Times.Exactly(4));
        transactionMock.Setup(p => p.Create(It.IsAny<LoginCredential>()))
            .Returns(ServiceResult.Ok)
            .Verifiable(Times.Once);
        transactionMock.Setup(p => p.CommitAsync())
            .ReturnsAsync(ServiceResult.Ok)
            .Verifiable(Times.Once);

        svcMock.Setup(p => p.GetTransaction())
            .Returns(transactionMock.Object)
            .Verifiable(Times.Once);
        svcMock.Setup(p => p.HashPasswordAsync<Pbkdf2Storage>(It.IsAny<SecureString>()))
            .ReturnsAsync([1, 2, 3, 4])
            .Verifiable(Times.Once);

        var result = await svcMock.Object.AddNewLoginCredential<Pbkdf2Storage>("test", "password".ToSecureString());

        Assert.That(result, Is.EqualTo(ServiceResult.Ok));
        Assert.That(differentGuids.Count, Is.EqualTo(4));
        transactionMock.Verify();
        svcMock.Verify();
    }

    [Test]
    public async Task AddNewLoginCredential_with_service_failure_at_commit_returns_failure()
    {
        var transactionMock = new Mock<ICrudReadWriteTransaction>();
        var svcMock = new Mock<IUserService>() { CallBase = true };
        LoginCredential? newCredential = null;

        transactionMock.Setup(p => p.SearchAsync(It.IsAny<Expression<Func<LoginCredential, bool>>>()))
            .ReturnsAsync(new ServiceResult<LoginCredential[]?>([]))
            .Verifiable(Times.Once);
        transactionMock.Setup(p => p.ReadAsync<LoginCredential, Guid>(It.IsAny<Guid>()))
            .ReturnsAsync(new ServiceResult<LoginCredential?>((LoginCredential?)null))
            .Verifiable(Times.AtLeastOnce);
        transactionMock.Setup(p => p.Create(It.IsAny<LoginCredential>()))
            .Callback<LoginCredential[]>(p => newCredential = p[0])
            .Returns(ServiceResult.Ok)
            .Verifiable(Times.Once);
        transactionMock.Setup(p => p.CommitAsync())
            .ReturnsAsync(FailureReason.ServiceFailure)
            .Verifiable(Times.Once);

        svcMock.Setup(p => p.GetTransaction())
            .Returns(transactionMock.Object)
            .Verifiable(Times.Once);
        svcMock.Setup(p => p.HashPasswordAsync<Pbkdf2Storage>(It.IsAny<SecureString>()))
            .ReturnsAsync([1, 2, 3, 4])
            .Verifiable(Times.Once);

        var result = await svcMock.Object.AddNewLoginCredential<Pbkdf2Storage>("test", "password".ToSecureString());

        Assert.That(result.Success, Is.False);
        Assert.That(result.Reason, Is.EqualTo(FailureReason.ServiceFailure));
        transactionMock.Verify();
        svcMock.Verify();
    }
}
