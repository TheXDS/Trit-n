#pragma  warning disable CS1591

using System;
using System.Collections.Generic;
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
        Assert.AreEqual(testId, x.Id);
        Assert.AreEqual("test1", x.Username);
        Assert.AreEqual(new byte[] { 1, 2, 4, 8 }, x.PasswordHash);
        Assert.IsFalse(x.Enabled);
        Assert.AreSame(sessionTest, x.Sessions);
        Assert.AreSame(mfaTest, x.RegisteredMfa);
        Assert.AreSame(descriptorsTest, x.Descriptors);
        Assert.AreSame(membershipTest, x.Membership);
        Assert.AreEqual(PermissionFlags.Special, x.Granted);
        Assert.AreEqual(PermissionFlags.ReadWrite, x.Revoked);
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
        Assert.AreEqual(testId, x.Id);
        Assert.AreEqual("Test group", x.DisplayName);
        Assert.AreEqual(PermissionFlags.Special, x.Granted);
        Assert.AreEqual(PermissionFlags.ReadWrite, x.Revoked);
        Assert.AreSame(descriptorsTest, x.Descriptors);
        Assert.AreSame(membershipTest, x.Membership);
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
            TtlHours = 48
        };
        Assert.AreEqual(testId, x.Id);
        Assert.AreSame(l, x.Credential);
        Assert.AreEqual("abcd1234", x.Token);
        Assert.AreEqual(48, x.TtlHours);
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
            Credential = l,
            Group = g
        };
        Assert.AreEqual(testId, x.Id);
        Assert.AreSame(l, x.Credential);
        Assert.AreSame(g, x.Group);
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
        Assert.AreEqual(testId, x.Id);
        Assert.AreEqual(PermissionFlags.Create, x.Granted);
        Assert.AreEqual(PermissionFlags.Delete, x.Revoked);
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
        Assert.AreEqual(testId, x.Id);
        Assert.AreSame(l, x.Credential);
        Assert.AreEqual(proc, x.MfaProcessor);
        Assert.AreEqual(data, x.Data);
    }
}