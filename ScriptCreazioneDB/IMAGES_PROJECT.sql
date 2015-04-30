USE [PLCCS_DB]
GO

/****** Object:  Table [dbo].[IMAGES_PROJECT]    Script Date: 29/04/2015 16:00:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[IMAGES_PROJECT](
	[ID] [varchar](50) NOT NULL,
	[Username] [int] NOT NULL,
	[Image] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_IMAGES_PROJECT] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO
