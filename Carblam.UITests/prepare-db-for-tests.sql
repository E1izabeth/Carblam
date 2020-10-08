/****** Object:  Table [dbo].[DbCarInfo]    Script Date: 06.06.2020 13:40:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbCarInfo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DriverUserId] [bigint] NOT NULL,
	[Designation] [nvarchar](4000) NULL,
	[Description] [nvarchar](4000) NULL,
	[LocationLat] [float] NOT NULL,
	[LocationLon] [float] NOT NULL,
	[CurrOrderId] [bigint] NOT NULL,
	[Width] [float] NOT NULL,
	[Length] [float] NOT NULL,
	[Height] [float] NOT NULL,
	[WeightLimit] [float] NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_DbCarInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbOrderInfo]    Script Date: 06.06.2020 13:40:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbOrderInfo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[RecieverId] [bigint] NOT NULL,
	[DriverId] [bigint] NOT NULL,
	[SourceAddress] [nvarchar](4000) NULL,
	[DestinationAddress] [nvarchar](4000) NULL,
	[SourceLocationLat] [float] NOT NULL,
	[SourceLocationLon] [float] NOT NULL,
	[DestinationLocationLat] [float] NOT NULL,
	[DestinationLocationLon] [float] NOT NULL,
	[Width] [float] NOT NULL,
	[Length] [float] NOT NULL,
	[Height] [float] NOT NULL,
	[Weight] [float] NOT NULL,
	[Status] [int] NOT NULL,
	[MessageForReciever] [nvarchar](4000) NULL,
	[Description] [nvarchar](4000) NULL,
	[CreatedStamp] [datetime] NOT NULL,
	[ConfirmedStamp] [datetime] NOT NULL,
	[AcceptedStamp] [datetime] NOT NULL,
	[LoadingStamp] [datetime] NOT NULL,
	[LoadedStamp] [datetime] NOT NULL,
	[DeliveredStamp] [datetime] NOT NULL,
	[FinishedStamp] [datetime] NOT NULL,
	[CancelledStamp] [datetime] NOT NULL,
 CONSTRAINT [PK_DbOrderInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DbUserInfo]    Script Date: 06.06.2020 13:40:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbUserInfo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Login] [nvarchar](150) NULL,
	[LoginKey] [nvarchar](150) NULL,
	[RegistrationStamp] [datetime] NOT NULL,
	[LastLoginStamp] [datetime] NOT NULL,
	[PasswordHash] [nvarchar](150) NULL,
	[HashSalt] [nvarchar](150) NULL,
	[Email] [nvarchar](150) NULL,
	[Activated] [bit] NOT NULL,
	[LastToken] [nvarchar](150) NULL,
	[LastTokenStamp] [datetime] NOT NULL,
	[LastTokenKind] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_DbUserInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[DbCarInfo] ON 
GO
INSERT [dbo].[DbCarInfo] ([Id], [DriverUserId], [Designation], [Description], [LocationLat], [LocationLon], [CurrOrderId], [Width], [Length], [Height], [WeightLimit], [Status]) VALUES (1, 1, N'к128ве', N'shevrolet lanos', 59.9357, 30.33743, -1, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[DbCarInfo] ([Id], [DriverUserId], [Designation], [Description], [LocationLat], [LocationLon], [CurrOrderId], [Width], [Length], [Height], [WeightLimit], [Status]) VALUES (2, 2, N'123567', N'', 60.015131666666669, 30.402938333333335, -1, 0, 0, 0, 0, 2)
GO
INSERT [dbo].[DbCarInfo] ([Id], [DriverUserId], [Designation], [Description], [LocationLat], [LocationLon], [CurrOrderId], [Width], [Length], [Height], [WeightLimit], [Status]) VALUES (3, 3, N'rk10009', N'blue', 60.014917, 30.4031091, 7, 0, 0, 0, 0, 1)
GO
INSERT [dbo].[DbCarInfo] ([Id], [DriverUserId], [Designation], [Description], [LocationLat], [LocationLon], [CurrOrderId], [Width], [Length], [Height], [WeightLimit], [Status]) VALUES (4, 4, N'lucky', N'black merc', 60.0150518, 30.4032295, 6, 0, 0, 0, 0, 2)
GO
SET IDENTITY_INSERT [dbo].[DbCarInfo] OFF
GO
SET IDENTITY_INSERT [dbo].[DbOrderInfo] ON 
GO
INSERT [dbo].[DbOrderInfo] ([Id], [CustomerId], [RecieverId], [DriverId], [SourceAddress], [DestinationAddress], [SourceLocationLat], [SourceLocationLon], [DestinationLocationLat], [DestinationLocationLon], [Width], [Length], [Height], [Weight], [Status], [MessageForReciever], [Description], [CreatedStamp], [ConfirmedStamp], [AcceptedStamp], [LoadingStamp], [LoadedStamp], [DeliveredStamp], [FinishedStamp], [CancelledStamp]) VALUES (1, 2, 2, 1, N'Russia, Sankt-Peterburg, Tsentralnyy rayon, Nevsky prospect, 76-68', N'Russia, Sankt-Peterburg, Barmaleyeva Ulitsa, 13', 59.933012, 30.3459401, 59.964522999999993, 30.305835400000003, 0, 0, 0, 0, 6, N'x', N'yellow', CAST(N'2020-06-04T19:54:07.033' AS DateTime), CAST(N'2020-06-05T10:54:07.033' AS DateTime), CAST(N'2020-06-05T23:43:12.037' AS DateTime), CAST(N'2020-06-05T23:43:18.740' AS DateTime), CAST(N'2020-06-05T23:43:23.900' AS DateTime), CAST(N'2020-06-05T23:43:34.760' AS DateTime), CAST(N'2020-06-05T23:43:41.427' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[DbOrderInfo] ([Id], [CustomerId], [RecieverId], [DriverId], [SourceAddress], [DestinationAddress], [SourceLocationLat], [SourceLocationLon], [DestinationLocationLat], [DestinationLocationLon], [Width], [Length], [Height], [Weight], [Status], [MessageForReciever], [Description], [CreatedStamp], [ConfirmedStamp], [AcceptedStamp], [LoadingStamp], [LoadedStamp], [DeliveredStamp], [FinishedStamp], [CancelledStamp]) VALUES (2, 1, 2, -1, N'Russia, Sankt-Peterburg, Tsentralnyy rayon, Nevsky Avenue, 71', N'Russia, Sankt-Peterburg, Zvozdnaya Ulitsa, 8', 59.931546999999995, 30.3548881, 59.833962899999989, 30.350338899999997, 0, 0, 0, 0, 0, NULL, NULL, CAST(N'2020-06-04T21:23:07.883' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[DbOrderInfo] ([Id], [CustomerId], [RecieverId], [DriverId], [SourceAddress], [DestinationAddress], [SourceLocationLat], [SourceLocationLon], [DestinationLocationLat], [DestinationLocationLon], [Width], [Length], [Height], [Weight], [Status], [MessageForReciever], [Description], [CreatedStamp], [ConfirmedStamp], [AcceptedStamp], [LoadingStamp], [LoadedStamp], [DeliveredStamp], [FinishedStamp], [CancelledStamp]) VALUES (3, 1, 2, -1, N'Россия, Санкт-Петербург, улица Бассейная, 41', N'Россия, Санкт-Петербург, Граждaнский проспект, д. 73', 59.864529, 30.322789, 60.0151693, 30.4026985, 0, 0, 0, 0, 0, NULL, N'secret', CAST(N'2020-06-05T20:56:03.050' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'2020-06-05T22:38:08.650' AS DateTime))
GO
INSERT [dbo].[DbOrderInfo] ([Id], [CustomerId], [RecieverId], [DriverId], [SourceAddress], [DestinationAddress], [SourceLocationLat], [SourceLocationLon], [DestinationLocationLat], [DestinationLocationLon], [Width], [Length], [Height], [Weight], [Status], [MessageForReciever], [Description], [CreatedStamp], [ConfirmedStamp], [AcceptedStamp], [LoadingStamp], [LoadedStamp], [DeliveredStamp], [FinishedStamp], [CancelledStamp]) VALUES (4, 3, 2, -1, N'Россия, Санкт-Петербург, Граждaнский проспект, д. 73', N'Россия, Санкт-Петербург, Кронверкский проспект, 49', 60.0151693, 30.4026985, 59.956401000000007, 30.309979, 0, 0, 0, 0, 0, NULL, N'docs', CAST(N'2020-06-05T20:59:05.490' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[DbOrderInfo] ([Id], [CustomerId], [RecieverId], [DriverId], [SourceAddress], [DestinationAddress], [SourceLocationLat], [SourceLocationLon], [DestinationLocationLat], [DestinationLocationLon], [Width], [Length], [Height], [Weight], [Status], [MessageForReciever], [Description], [CreatedStamp], [ConfirmedStamp], [AcceptedStamp], [LoadingStamp], [LoadedStamp], [DeliveredStamp], [FinishedStamp], [CancelledStamp]) VALUES (5, 3, 3, 1, N'Россия, Санкт-Петербург, Граждaнский проспект, д. 73', N'Россия, Санкт-Петербург, Вяземский переулок, 5/7', 60.0151693, 30.4026985, 59.972875599999995, 30.301564700000004, 0, 0, 0, 0, 6, NULL, N'blueberry', CAST(N'2020-06-05T21:03:04.340' AS DateTime), CAST(N'2020-06-05T21:03:35.057' AS DateTime), CAST(N'2020-06-06T00:54:01.220' AS DateTime), CAST(N'2020-06-06T00:54:08.100' AS DateTime), CAST(N'2020-06-06T01:08:14.630' AS DateTime), CAST(N'2020-06-06T01:08:16.880' AS DateTime), CAST(N'2020-06-06T01:08:18.300' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[DbOrderInfo] ([Id], [CustomerId], [RecieverId], [DriverId], [SourceAddress], [DestinationAddress], [SourceLocationLat], [SourceLocationLon], [DestinationLocationLat], [DestinationLocationLon], [Width], [Length], [Height], [Weight], [Status], [MessageForReciever], [Description], [CreatedStamp], [ConfirmedStamp], [AcceptedStamp], [LoadingStamp], [LoadedStamp], [DeliveredStamp], [FinishedStamp], [CancelledStamp]) VALUES (6, 3, 1, 4, N'Россия, Санкт-Петербург, Граждaнский проспект, д. 73', N'Россия, Санкт-Петербург, Вяземский переулок, 5/7', 60.0151693, 30.4026985, 59.972875599999995, 30.301564700000004, 0, 0, 0, 0, 6, NULL, N'apples', CAST(N'2020-06-05T21:07:20.790' AS DateTime), CAST(N'2020-06-05T21:58:22.183' AS DateTime), CAST(N'2020-06-05T23:39:21.617' AS DateTime), CAST(N'2020-06-05T23:40:18.333' AS DateTime), CAST(N'2020-06-05T23:40:31.807' AS DateTime), CAST(N'2020-06-05T23:40:36.490' AS DateTime), CAST(N'2020-06-05T23:40:40.987' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[DbOrderInfo] ([Id], [CustomerId], [RecieverId], [DriverId], [SourceAddress], [DestinationAddress], [SourceLocationLat], [SourceLocationLon], [DestinationLocationLat], [DestinationLocationLon], [Width], [Length], [Height], [Weight], [Status], [MessageForReciever], [Description], [CreatedStamp], [ConfirmedStamp], [AcceptedStamp], [LoadingStamp], [LoadedStamp], [DeliveredStamp], [FinishedStamp], [CancelledStamp]) VALUES (7, 2, 3, 3, N'Россия, Санкт-Петербург, Большой Сампсониевский проспект, д.21', N'Россия, Санкт-Петербург, Торфяная дорога, 2 корпус 1', 59.964335899999995, 30.344473099999995, 59.988653799999994, 30.254833499999997, 0, 0, 0, 0, 5, N'a little prince', N'a book', CAST(N'2020-06-05T23:45:57.017' AS DateTime), CAST(N'2020-06-06T01:02:29.177' AS DateTime), CAST(N'2020-06-06T01:02:40.697' AS DateTime), CAST(N'2020-06-06T01:03:11.460' AS DateTime), CAST(N'2020-06-06T01:03:56.250' AS DateTime), CAST(N'2020-06-06T01:08:33.677' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[DbOrderInfo] ([Id], [CustomerId], [RecieverId], [DriverId], [SourceAddress], [DestinationAddress], [SourceLocationLat], [SourceLocationLon], [DestinationLocationLat], [DestinationLocationLon], [Width], [Length], [Height], [Weight], [Status], [MessageForReciever], [Description], [CreatedStamp], [ConfirmedStamp], [AcceptedStamp], [LoadingStamp], [LoadedStamp], [DeliveredStamp], [FinishedStamp], [CancelledStamp]) VALUES (8, 2, 3, 1, N'Россия, Санкт-Петербург, Граждaнский проспект, д. 73', N'Россия, Санкт-Петербург, улица Бассейная, 41', 60.0151693, 30.4026985, 59.864529, 30.322789, 0, 0, 0, 0, 6, N'ku', N'test', CAST(N'2020-06-06T01:34:38.587' AS DateTime), CAST(N'2020-06-06T01:34:51.037' AS DateTime), CAST(N'2020-06-06T01:40:09.647' AS DateTime), CAST(N'2020-06-06T01:49:28.240' AS DateTime), CAST(N'2020-06-06T02:04:15.927' AS DateTime), CAST(N'2020-06-06T02:05:01.597' AS DateTime), CAST(N'2020-06-06T02:05:08.013' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[DbOrderInfo] ([Id], [CustomerId], [RecieverId], [DriverId], [SourceAddress], [DestinationAddress], [SourceLocationLat], [SourceLocationLon], [DestinationLocationLat], [DestinationLocationLon], [Width], [Length], [Height], [Weight], [Status], [MessageForReciever], [Description], [CreatedStamp], [ConfirmedStamp], [AcceptedStamp], [LoadingStamp], [LoadedStamp], [DeliveredStamp], [FinishedStamp], [CancelledStamp]) VALUES (9, 1, 2, 1, N'Russia, Sankt-Peterburg, Ulitsa Basseynaya, 41', N'Russia, Sankt-Peterburg, Grazhdanskiy Prospekt, 73', 59.864529, 30.322789, 60.0151693, 30.4026985, 0, 0, 0, 0, 6, N'some secret message', N'my testful parcel', CAST(N'2020-06-06T07:11:45.177' AS DateTime), CAST(N'2020-06-06T07:39:00.223' AS DateTime), CAST(N'2020-06-06T07:45:39.563' AS DateTime), CAST(N'2020-06-06T07:45:48.620' AS DateTime), CAST(N'2020-06-06T07:45:52.847' AS DateTime), CAST(N'2020-06-06T07:45:57.120' AS DateTime), CAST(N'2020-06-06T07:46:00.477' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[DbOrderInfo] ([Id], [CustomerId], [RecieverId], [DriverId], [SourceAddress], [DestinationAddress], [SourceLocationLat], [SourceLocationLon], [DestinationLocationLat], [DestinationLocationLon], [Width], [Length], [Height], [Weight], [Status], [MessageForReciever], [Description], [CreatedStamp], [ConfirmedStamp], [AcceptedStamp], [LoadingStamp], [LoadedStamp], [DeliveredStamp], [FinishedStamp], [CancelledStamp]) VALUES (10, 1, 2, 1, N'Russia, Sankt-Peterburg, Ulitsa Basseynaya, 41', N'Russia, Sankt-Peterburg, Grazhdanskiy Prospekt, 73', 59.864529, 30.322789, 60.0151693, 30.4026985, 0, 0, 0, 0, 6, N'some secret message', N'my testful parcel', CAST(N'2020-06-06T08:29:45.117' AS DateTime), CAST(N'2020-06-06T08:31:05.110' AS DateTime), CAST(N'2020-06-06T08:32:21.243' AS DateTime), CAST(N'2020-06-06T08:32:33.743' AS DateTime), CAST(N'2020-06-06T08:32:36.577' AS DateTime), CAST(N'2020-06-06T08:32:40.380' AS DateTime), CAST(N'2020-06-06T08:32:43.017' AS DateTime), CAST(N'1753-01-01T00:00:00.000' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[DbOrderInfo] OFF
GO
SET IDENTITY_INSERT [dbo].[DbUserInfo] ON 
GO
INSERT [dbo].[DbUserInfo] ([Id], [Login], [LoginKey], [RegistrationStamp], [LastLoginStamp], [PasswordHash], [HashSalt], [Email], [Activated], [LastToken], [LastTokenStamp], [LastTokenKind], [IsDeleted]) VALUES (1, N'login', N'login', CAST(N'2020-05-24T19:52:38.453' AS DateTime), CAST(N'2020-06-06T08:35:50.520' AS DateTime), N'JNQpnV6yqgSaARdfBGT+VrskqCMaLBBScIZL53yR+ic=', N'R+O5zFGg3wRc9zEp5ywVUW0XZ8sU4UQ+Q51xDKsayrf5lWrGKnMn9quHNC90ihR0WxOULqYPJZO6pWQ1qYfTQQ==', N'email@mail.ru', 1, NULL, CAST(N'2020-05-31T13:48:10.610' AS DateTime), 1, 0)
GO
INSERT [dbo].[DbUserInfo] ([Id], [Login], [LoginKey], [RegistrationStamp], [LastLoginStamp], [PasswordHash], [HashSalt], [Email], [Activated], [LastToken], [LastTokenStamp], [LastTokenKind], [IsDeleted]) VALUES (2, N'e1izabeth', N'e1izabeth', CAST(N'2020-05-31T15:20:48.083' AS DateTime), CAST(N'2020-06-06T08:30:46.033' AS DateTime), N'V5geJ6syQ4Knl6yXER+waXDwXi1CXONON61lzIx5pHU=', N'Z9aysnifFuk5WneriayPRHy/d7rz0GKDIGc2+fWEBO+dl1pjWZHAlU7zRGmmWovmdwYgNqt5c1647In49G03cw==', N'kuzenkova@e1izabeth.online', 1, NULL, CAST(N'2020-05-31T17:50:37.443' AS DateTime), 1, 0)
GO
INSERT [dbo].[DbUserInfo] ([Id], [Login], [LoginKey], [RegistrationStamp], [LastLoginStamp], [PasswordHash], [HashSalt], [Email], [Activated], [LastToken], [LastTokenStamp], [LastTokenKind], [IsDeleted]) VALUES (3, N'nokia', N'nokia', CAST(N'2020-06-05T20:51:25.897' AS DateTime), CAST(N'2020-06-06T07:57:52.050' AS DateTime), N'95luXbr11JShPljpBNNrGvK0MGJ/g5yr2lHcHY/2nrw=', N'oMmd8SZb12kVZzfrilF6Do/YeDSHH7Vm0WWHBg0CJPmcfvdLs401n0XMnnV1vxXzMVjYt2vgOxWvUgE3Aig0Nw==', N'kuzenkova.el@yandex.ru', 1, NULL, CAST(N'2020-06-05T20:51:25.967' AS DateTime), 0, 0)
GO
INSERT [dbo].[DbUserInfo] ([Id], [Login], [LoginKey], [RegistrationStamp], [LastLoginStamp], [PasswordHash], [HashSalt], [Email], [Activated], [LastToken], [LastTokenStamp], [LastTokenKind], [IsDeleted]) VALUES (4, N'tester', N'tester', CAST(N'2020-06-05T22:37:38.977' AS DateTime), CAST(N'2020-06-05T23:38:52.647' AS DateTime), N'qn4Yj1kZ0mpkJA5VxyIeUYxo2/klzwJluhjdtUMkKUM=', N'49hyWatlKWuxmfkPGP/f96B01ljkuvpUPEj/aRqHVAutxi0spsR1C47QX8pP3T1PBrYELbwx6n+IMXjmx1AL7w==', N'kuzenkova.el@yandex.ru', 1, NULL, CAST(N'2020-06-05T23:02:37.140' AS DateTime), 0, 0)
GO
SET IDENTITY_INSERT [dbo].[DbUserInfo] OFF
GO
USE [master]
GO
