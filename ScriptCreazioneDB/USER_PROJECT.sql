USE [PLCCS_DB]
GO

/****** Object:  Table [dbo].[USERS_PROJECT]    Script Date: 29/04/2015 16:00:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[USERS_PROJECT](
	[Username] [int] NOT NULL,
	[PasswordLogin] [varchar](20) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Surname] [varchar](50) NOT NULL,
	[Gender] [nchar](1) NOT NULL,
	[BirthDate] [date] NOT NULL,
	[Administrator] [nchar](1) NOT NULL,
	[MailAddress] [varchar](50) NULL,
	[MailPassword] [varchar](100) NULL,
	[Image] [varbinary](max) NULL,
 CONSTRAINT [PK_USERS_PROJECT] PRIMARY KEY CLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'''M'' indica persona di sesso maschile, ''F'' indica una persona di sesso femminile' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'USERS_PROJECT', @level2type=N'COLUMN',@level2name=N'Gender'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'''S'' se si tratta di un amministratore, ''N'' altrimenti' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'USERS_PROJECT', @level2type=N'COLUMN',@level2name=N'Administrator'
GO

