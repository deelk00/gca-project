namespace DynamicQL.Authentication.Ldap.Utility
{
    /// <summary>
    ///     Most common LDAP properties stored in a class
    ///     for better readability
    /// </summary>
    public static class LdapProperties
    {
        public static readonly string OBJECTCLASS = "objectClass";
        public static readonly string CONTAINERNAME = "cn";
        public static readonly string LASTNAME = "sn";
        public static readonly string COUNTRYNOTATION = "c";
        public static readonly string CITY = "l";
        public static readonly string STATE = "st";
        public static readonly string TITLE = "title";
        public static readonly string POSTALCODE = "postalCode";
        public static readonly string PHYSICALDELIVERYOFFICENAME = "physicalDeliveryOfficeName";
        public static readonly string FIRSTNAME = "givenName";
        public static readonly string MIDDLENAME = "initials";
        public static readonly string DISTINGUISHEDNAME = "distinguishedName";
        public static readonly string INSTANCETYPE = "instanceType";
        public static readonly string WHENCREATED = "whenCreated";
        public static readonly string WHENCHANGED = "whenChanged";
        public static readonly string DISPLAYNAME = "displayName";
        public static readonly string USNCREATED = "uSNCreated";
        public static readonly string MEMBEROF = "memberOf";
        public static readonly string USNCHANGED = "uSNChanged";
        public static readonly string COUNTRY = "co";
        public static readonly string DEPARTMENT = "department";
        public static readonly string COMPANY = "company";
        public static readonly string PROXYADDRESSES = "proxyAddresses";
        public static readonly string STREETADDRESS = "streetAddress";
        public static readonly string DIRECTREPORTS = "directReports";
        public static readonly string NAME = "name";
        public static readonly string OBJECTGUID = "objectGUID";
        public static readonly string USERACCOUNTCONTROL = "userAccountControl";
        public static readonly string BADPWDCOUNT = "badPwdCount";
        public static readonly string CODEPAGE = "codePage";
        public static readonly string COUNTRYCODE = "countryCode";
        public static readonly string BADPASSWORDTIME = "badPasswordTime";
        public static readonly string LASTLOGOFF = "lastLogoff";
        public static readonly string LASTLOGON = "lastLogon";
        public static readonly string PWDLASTSET = "pwdLastSet";
        public static readonly string PRIMARYGROUPID = "primaryGroupID";
        public static readonly string OBJECTSID = "objectSid";
        public static readonly string ADMINCOUNT = "adminCount";
        public static readonly string ACCOUNTEXPIRES = "accountExpires";
        public static readonly string LOGONCOUNT = "logonCount";
        public static readonly string LOGINNAME = "sAMAccountName";
        public static readonly string SAMACCOUNTNAME = "sAMAccountName";
        public static readonly string SAMACCOUNTTYPE = "sAMAccountType";
        public static readonly string SHOWINADDRESSBOOK = "showInAddressBook";
        public static readonly string LEGACYEXCHANGEDN = "legacyExchangeDN";
        public static readonly string USERPRINCIPALNAME = "userPrincipalName";
        public static readonly string EXTENSION = "ipPhone";
        public static readonly string SERVICEPRINCIPALNAME = "servicePrincipalName";
        public static readonly string OBJECTCATEGORY = "objectCategory";
        public static readonly string DSCOREPROPAGATIONDATA = "dSCorePropagationData";
        public static readonly string LASTLOGONTIMESTAMP = "lastLogonTimestamp";
        public static readonly string EMAILADDRESS = "mail";
        public static readonly string MANAGER = "manager";
        public static readonly string MOBILE = "mobile";
        public static readonly string PAGER = "pager";
        public static readonly string FAX = "facsimileTelephoneNumber";
        public static readonly string HOMEPHONE = "homePhone";
        public static readonly string MSEXCHUSERACCOUNTCONTROL = "msExchUserAccountControl";
        public static readonly string MDBUSEDEFAULTS = "mDBUseDefaults";
        public static readonly string MSEXCHMAILBOXSECURITYDESCRIPTOR = "msExchMailboxSecurityDescriptor";
        public static readonly string HOMEMDB = "homeMDB";
        public static readonly string MSEXCHPOLICIESINCLUDED = "msExchPoliciesIncluded";
        public static readonly string HOMEMTA = "homeMTA";
        public static readonly string MSEXCHRECIPIENTTYPEDETAILS = "msExchRecipientTypeDetails";
        public static readonly string MAILNICKNAME = "mailNickname";
        public static readonly string MSEXCHHOMESERVERNAME = "msExchHomeServerName";
        public static readonly string MSEXCHVERSION = "msExchVersion";
        public static readonly string MSEXCHRECIPIENTDISPLAYTYPE = "msExchRecipientDisplayType";
        public static readonly string MSEXCHMAILBOXGUID = "msExchMailboxGuid";
        public static readonly string NTSECURITYDESCRIPTOR = "nTSecurityDescriptor";
    }
}