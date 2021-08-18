﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

public partial class Average
{
    public int Id { get; set; }
    public int Player { get; set; }
    public int Legs { get; set; }
    public int Pins { get; set; }
    public int Game { get; set; }
    public decimal Avg { get; set; }
}

public partial class Competition
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Nullable<bool> Matchplay { get; set; }
    public bool Locked { get; set; }
    public Nullable<bool> KeepAverages { get; set; }
    public bool Archive { get; set; }
}

public partial class Document
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Tag { get; set; }
}

public partial class DocumentStorage
{
    public int Id { get; set; }
    public byte[] Content { get; set; }
}

public partial class Game
{
    public int Id { get; set; }
    public string Opponents { get; set; }
    public System.DateTime Date { get; set; }
    public int Competition { get; set; }
    public Nullable<int> Set1 { get; set; }
    public Nullable<int> Set2 { get; set; }
    public Nullable<bool> Late { get; set; }
    public Nullable<bool> InProgress { get; set; }
    public int Us { get; set; }
    public int Them { get; set; }
    public bool Complete { get; set; }
    public Nullable<bool> Lane { get; set; }
}

public partial class Score
{
    public int Id { get; set; }
    public int Player { get; set; }
    public int Leg1 { get; set; }
    public int Leg2 { get; set; }
    public int Leg3 { get; set; }
    public decimal Points1 { get; set; }
    public decimal Points2 { get; set; }
    public decimal Points3 { get; set; }
}

public partial class Set
{
    public int Id { get; set; }
    public int Scores1 { get; set; }
    public int Scores2 { get; set; }
    public int Scores3 { get; set; }
    public int Scores4 { get; set; }
    public int Scores5 { get; set; }
    public int Scores6 { get; set; }
}

public partial class UserProfile
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public UserProfile()
    {
        this.webpages_Roles = new HashSet<webpages_Roles>();
    }

    public string Email { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; }
    public string TeamRole { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    public virtual ICollection<webpages_Roles> webpages_Roles { get; set; }
}

public partial class webpages_Membership
{
    public int UserId { get; set; }
    public Nullable<System.DateTime> CreateDate { get; set; }
    public string ConfirmationToken { get; set; }
    public Nullable<bool> IsConfirmed { get; set; }
    public Nullable<System.DateTime> LastPasswordFailureDate { get; set; }
    public int PasswordFailuresSinceLastSuccess { get; set; }
    public string Password { get; set; }
    public Nullable<System.DateTime> PasswordChangedDate { get; set; }
    public string PasswordSalt { get; set; }
    public string PasswordVerificationToken { get; set; }
    public Nullable<System.DateTime> PasswordVerificationTokenExpirationDate { get; set; }
}

public partial class webpages_OAuthMembership
{
    public string Provider { get; set; }
    public string ProviderUserId { get; set; }
    public int UserId { get; set; }
}

public partial class webpages_Roles
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public webpages_Roles()
    {
        this.UserProfiles = new HashSet<UserProfile>();
    }

    public int RoleId { get; set; }
    public string RoleName { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    public virtual ICollection<UserProfile> UserProfiles { get; set; }
}
