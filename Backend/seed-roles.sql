INSERT INTO [BookMS].[dbo].[Roles] ([Id], [Name], [CreatedAt], [IsDeleted])
SELECT NEWID(), 'User', GETUTCDATE(), 0
WHERE NOT EXISTS (SELECT 1 FROM [BookMS].[dbo].[Roles] WHERE [Name] = 'User');

INSERT INTO [BookMS].[dbo].[Roles] ([Id], [Name], [CreatedAt], [IsDeleted])
SELECT NEWID(), 'Admin', GETUTCDATE(), 0
WHERE NOT EXISTS (SELECT 1 FROM [BookMS].[dbo].[Roles] WHERE [Name] = 'Admin');

INSERT INTO [BookMS].[dbo].[Roles] ([Id], [Name], [CreatedAt], [IsDeleted])
SELECT NEWID(), 'SuperAdmin', GETUTCDATE(), 0
WHERE NOT EXISTS (SELECT 1 FROM [BookMS].[dbo].[Roles] WHERE [Name] = 'SuperAdmin');
