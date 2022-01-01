#Mandatory
#Master Database migration  
--NO NEED TO ADD: add-migration -Context MasterDBContext  init
update-database -Context MasterDBContext  

#Run on POCMaster_db Database
INSERT POCMaster_db.[dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'1', N'Admin', N'Admin', NULL)
GO
INSERT POCMaster_db.[dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp]) VALUES (N'2', N'User', N'User', NULL)
GO




#Optional
Shared Database migration
add-migration -Context SharedDBContext  inits
update-database -Context SharedDBContext  

#there is no Dedicated db to migrate. Dedicated db will be created on the runtime programatically.


#Sample API call object

{
  "fullName": "iqbal",
  "email": "aslamcpp@gmail.com",
  "password": "M!longP@$$word",
  "paymentAmount": 10,
  "favoriteProgrammingLanguages": "C#, T-SQL",
  "favoriteIDEs": "VS2019"
}