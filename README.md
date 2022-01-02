#Create a database POCMaster_db

#set password in appsetting.json

--Migration database

#update-database -Context MasterDBContext  


--test

#Sample API call object

{

  "fullName": "iqbal",
  
  "email": "aslamcpp@gmail.com",
  
  "password": "M!longP@$$word",
  
  "paymentAmount": 10,
  
  "favoriteProgrammingLanguages": "C#, T-SQL",
  
  "favoriteIDEs": "VS2019"
  
}


SELECT *  FROM [POCShared_db].[dbo].[FavoritePL]


SELECT *  FROM [aslamcpp].[dbo].[FavoritePL]
