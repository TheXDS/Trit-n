#pragma  warning disable CS1591

using System;
using System.Collections.Generic;
using NUnit.Framework;
using TheXDS.Triton.Models;

namespace TheXDS.Triton.Tests.SecurityEssentials;

public class ModelsTests
{
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
}