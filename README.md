#set password in appsetting.json
--Migration database
#update-database -Context MasterDBContext  


--test
SELECT *  FROM [POCShared_db].[dbo].[FavoritePL]
SELECT *  FROM [aslamcpp].[dbo].[FavoritePL]
