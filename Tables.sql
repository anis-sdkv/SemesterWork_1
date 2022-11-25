CREATE TABLE Users(
	id INT NOT NULL IDENTITY(1, 1),
	nickname VARCHAR(50) NOT NULL,	
	[login] VARCHAR(50) NOT NULL,
	[password] VARCHAR(30) NOT NULL,
	[role] VARCHAR(1) NOT NULL,
	first_name VARCHAR(50),
	last_name VARCHAR(50),
	patronymic VARCHAR(50),

	CONSTRAINT PR_user PRIMARY KEY(id),
	CONSTRAINT CK_user_role CHECK ([role] IN('r','a', 'm')),  -- reader/author/moderator
)

CREATE TABLE Categories(
	id INT IDENTITY(1, 1),
	[name] VARCHAR(30),

	CONSTRAINT PR_category PRIMARY KEY(id),
)

CREATE TABLE Publications(
	id INT NOT NULL IDENTITY(1, 1),
	author_id INT NOT NULL,
	[date] DATETIME NOT NULL,
	[status] VARCHAR(1) NOT NULL,
	category_id INT NOT NULL,

	CONSTRAINT CK_user_role CHECK ([status] IN('p','m')),  -- public/on moderation
	CONSTRAINT PR_publication PRIMARY KEY(id),
	CONSTRAINT FK_publication_to_author FOREIGN KEY (author_id)
		REFERENCES Author(id),
	CONSTRAINT FK_publication_to_category FOREIGN KEY (author_id)
		REFERENCES Categories(id),
)

CREATE TABLE Comments(
	id INT NOT NULL IDENTITY(1, 1),
	publication_id INT NOT NULL,
	[user_id] INT NOT NULL,
	[text] VARCHAR(500) NOT NULL,
	[datet] DATETIME NOT NULL,

	CONSTRAINT PR_comment PRIMARY KEY(id),
	CONSTRAINT FK_comment_to_user FOREIGN KEY (user_id)
		REFERENCES Users(id),
	CONSTRAINT FK_comment_to_publication FOREIGN KEY (publication_id)
		REFERENCES Publications(id)
)

CREATE TABLE Report(
	id INT NOT NULL IDENTITY(1,1),
	[user_id] INT NOT NULL,
	[text] VARCHAR(500) NOT NULL, 

	CONSTRAINT PR_Report PRIMARY KEY(id),
	CONSTRAINT FK_report_to_user FOREIGN KEY ([user_id])
		REFERENCES Users(id)
)