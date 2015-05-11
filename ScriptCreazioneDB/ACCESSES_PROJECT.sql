USE [PLCCS_DB]
GO

/****** Object:  Table [dbo].[ACCESSES_PROJECT]    Script Date: 05/05/2015 15:35:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

ALTER TABLE [dbo].[ACCESSES_PROJECT](
	[Username] [int] NOT NULL,
	[DateAccess] [datetime] NOT NULL,
	[TypeOfAccess] [nchar](1) NOT NULL,
	[ImageAccess] [varbinary](max) NULL,
 CONSTRAINT [pk_ACCESSES_PROJECT] PRIMARY KEY CLUSTERED 
(
	[Username] ASC,
	[DateAccess] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'''L'' per un Login Tradizionale, ''F'' per un accesso tramite FaceRecognition' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ACCESSES_PROJECT', @level2type=N'COLUMN',@level2name=N'TypeOfAccess'
GO


