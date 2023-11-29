#pragma  warning disable CS1591

using NUnit.Framework;
using TheXDS.Triton.Models;

namespace TheXDS.Triton.Tests.SecurityEssentials;

public class ModelsTests
{
    private class SecurityBaseTestClass : SecurityBase
    {
    }
    
    [Test]
    public void LoginCredential_Test()
    {
        Guid testId = Guid.NewGuid();
        List<Session> sessionTest = new();
        List<MultiFactorEntry> mfaTest = new();
        List<SecurityDescriptor> descriptorsTest = new();
        List<UserGroupMembership> membershipTest = new();

        LoginCredential x = new()
        {
            Id = testId,
            Username = "test1",
            PasswordHash = new byte[] { 1, 2, 4, 8 },
            Enabled = false,
            Sessions = sessionTest,
            RegisteredMfa = mfaTest,
            Descriptors = descriptorsTest,
            Membership = membershipTest,
            Granted = PermissionFlags.Special,
            Revoked = PermissionFlags.ReadWrite
        };
        Assert.That(x.Id, Is.EqualTo(testId));
        Assert.That(x.Username, Is.EqualTo("test1"));
        Assert.That(x.PasswordHash, Is.EquivalentTo(new byte[] { 1, 2, 4, 8 }));
        Assert.That(x.Enabled, Is.False);
        Assert.That(x.Sessions, Is.SameAs(sessionTest));
        Assert.That(x.RegisteredMfa, Is.SameAs(mfaTest));
        Assert.That(x.Descriptors, Is.SameAs(descriptorsTest));
        Assert.That(x.Membership, Is.SameAs(membershipTest));
        Assert.That(x.Granted, Is.EqualTo(PermissionFlags.Special));
        Assert.That(x.Revoked, Is.EqualTo(PermissionFlags.ReadWrite));
    }

    [Test]
    public void UserGroup_Test()
    {
        Guid testId = Guid.NewGuid();
        List<UserGroupMembership> membershipTest = new();
        List<SecurityDescriptor> descriptorsTest = new();
        UserGroup x = new()
        {
            Id = testId,
            DisplayName = "Test group",
            Granted = PermissionFlags.Special,
            Revoked = PermissionFlags.ReadWrite,
            Membership = membershipTest,
            Descriptors = descriptorsTest,
        };
        Assert.That(x.Id, Is.EqualTo(testId));
        Assert.That(x.DisplayName, Is.EqualTo("Test group"));
        Assert.That(x.Granted, Is.EqualTo(PermissionFlags.Special));
        Assert.That(x.Revoked, Is.EqualTo(PermissionFlags.ReadWrite));
        Assert.That(x.Descriptors, Is.SameAs(descriptorsTest));
        Assert.That(x.Membership, Is.SameAs(membershipTest));
    }

    [Test]
    public void Session_Test()
    {
        Guid testId = Guid.NewGuid();
        LoginCredential l = new();
        Session x = new()
        {
            Id = testId,
            Credential = l,
            Token = "abcd1234",
            TtlSeconds = 48
        };
        Assert.That(x.Id, Is.EqualTo(testId));
        Assert.That(x.Credential, Is.SameAs(l));
        Assert.That(x.Token, Is.EqualTo("abcd1234"));
        Assert.That(x.TtlSeconds, Is.EqualTo(48));
    }

    [Test]
    public void UserGroupMembership_Test()
    {
        Guid testId = Guid.NewGuid();
        LoginCredential l = new();
        UserGroup g = new();
        UserGroupMembership x = new()
        {
            Id = testId,
            SecurityObject = l,
            Group = g
        };
        Assert.That(x.Id, Is.EqualTo(testId));
        Assert.That(x.SecurityObject, Is.SameAs(l));
        Assert.That(x.Group, Is.SameAs(g));
    }

    [Test]
    public void SecurityBase_Test()
    {
        Guid testId = Guid.NewGuid();
        SecurityBaseTestClass x = new()
        {
            Id = testId,
            Granted = PermissionFlags.Create,
            Revoked = PermissionFlags.Delete
        };
        Assert.That(x.Id, Is.EqualTo(testId));
        Assert.That(x.Granted, Is.EqualTo(PermissionFlags.Create));
        Assert.That(x.Revoked, Is.EqualTo(PermissionFlags.Delete));
    }

    [Test]
    public void MultiFactorEntry_Test()
    {
        Guid testId = Guid.NewGuid();
        Guid proc = Guid.NewGuid();
        byte[] data = { 1, 2, 3, 4 };
        LoginCredential l = new();
        MultiFactorEntry x = new()
        {
            Id = testId,
            Credential = l,
            MfaProcessor = proc,
            Data = data
        };
        Assert.That(x.Id, Is.EqualTo(testId));
        Assert.That(x.Credential, Is.SameAs(l));
        Assert.That(x.MfaProcessor, Is.EqualTo(proc));
        Assert.That(x.Data, Is.EqualTo(data));
    }
}