using System;
using CRUDUsersApi;

namespace Tests;

public class ValidateTest
{
    UserValidate validate = new UserValidate();

    [Fact]
    public void PasswordLoginValidateTest()
    {
        string l1 = "MrBrrown";
        string l2 = "МистерБраун";
        string l3 = "MrBrrown2019";
        string l4 = "MrBrrаун2019";
        string l5 = "MrBrrown__1";
        string l6 = "MrBrrown 2019";
        string l7 = "";

        Assert.True(validate.PasswordLoginValidate(l1));
        Assert.False(validate.PasswordLoginValidate(l2));
        Assert.True(validate.PasswordLoginValidate(l3));
        Assert.False(validate.PasswordLoginValidate(l4));
        Assert.False(validate.PasswordLoginValidate(l5));
        Assert.False(validate.PasswordLoginValidate(l6));
        Assert.False(validate.PasswordLoginValidate(l7));
    }

    [Fact]
    public void NameValidateTest()
    {
        string l1 = "Richard";
        string l2 = "Ричерд";
        string l3 = "ричерд";
        string l4 = "Richerd2009";
        string l5 = "Ричерд--!!";
        string l6 = "RicЁрд";
        string l7 = "";

        Assert.True(validate.NameValidate(l1));
        Assert.True(validate.NameValidate(l2));
        Assert.True(validate.NameValidate(l3));
        Assert.False(validate.NameValidate(l4));
        Assert.False(validate.NameValidate(l5));
        Assert.True(validate.NameValidate(l6));
        Assert.False(validate.NameValidate(l7));
    }

    [Fact]
    public void BirthdayValidateTest()
    {
        DateTime dateTime1 = new DateTime(2025, 12, 01);
        DateTime dateTime2 = new DateTime(2020, 01, 01);
        DateTime? dateTime3 = null;

        Assert.False(validate.BirthdayValidate(dateTime1));
        Assert.True(validate.BirthdayValidate(dateTime2));
        Assert.True(validate.BirthdayValidate(dateTime3));
    }

    [Fact]
    public void GenderValidateTest()
    {
        int gender1 = 0;
        int gender2 = 1;
        int gender3 = 2;
        int gender4 = 3;

        Assert.True(validate.GenderValidate(gender1));
        Assert.True(validate.GenderValidate(gender2));
        Assert.True(validate.GenderValidate(gender3));
        Assert.False(validate.GenderValidate(gender4));
    }
}
