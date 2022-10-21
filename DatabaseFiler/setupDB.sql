DROP DATABASE IF EXISTS propositio;
CREATE DATABASE propositio;
USE propositio;
SET NAMES 'utf8' COLLATE 'utf8_danish_ci';
DROP TABLE IF EXISTS Employee,
Team,
TeamList,
Role,
SuggestionComment,
Suggestion,
SuggestionTimestamp,
CategoryList,
Category,
AuthenticationRole,
EmployeeAuthenticationRole;
CREATE TABLE Role(
    role_id int NOT NULL PRIMARY KEY AUTO_INCREMENT,
    role_name varchar(100) NOT NULL
);
CREATE TABLE AuthorizationRole(
    authorization_role_id int NOT NULL PRIMARY KEY AUTO_INCREMENT,
    authorization_role_name nvarchar(100) NOT NULL
);
CREATE TABLE Employee(
    emp_id int NOT NULL PRIMARY KEY,
    name nvarchar(200) NOT NULL,
    passwordhash nvarchar(200) NOT NULL,
    salt binary(64) NOT NULL,
    role_id int NOT NULL,
    authorization_role_id int NOT NULL,
    CONSTRAINT authRoleFK FOREIGN KEY (authorization_role_id) REFERENCES AuthorizationRole(authorization_role_id),
    CONSTRAINT roleFK FOREIGN KEY (role_id) REFERENCES Role(role_id)
);
CREATE TABLE Team(
    team_id int NOT NULL PRIMARY KEY AUTO_INCREMENT,
    team_name nvarchar(100) NOT NULL,
    team_lead_id int NOT NULL
);
CREATE TABLE TeamList(
    emp_id int NOT NULL,
    team_id int NOT NULL,
    CONSTRAINT EmployeeFK FOREIGN KEY (emp_id) REFERENCES Employee(emp_id),
    CONSTRAINT TeamFK FOREIGN KEY (team_id) REFERENCES Team(team_id),
    CONSTRAINT TeamListPK PRIMARY KEY (emp_id, team_id)
);
CREATE TABLE Suggestion(
    suggestion_id int NOT NULL PRIMARY KEY AUTO_INCREMENT,
    title nvarchar(100) NOT NULL,
    description nvarchar(6000) NOT NULL,
    status enum(
        "PLAN",
        "DO",
        "STUDY",
        "ACT",
        "FINISHED",
        "JUSTDOIT"
    ) NOT NULL,
    ownership_emp_id int NOT NULL,
    author_emp_id int NOT NULL,
    CONSTRAINT OwnershipFK FOREIGN KEY (ownership_emp_id) REFERENCES Employee(emp_id),
    CONSTRAINT PosterFK FOREIGN KEY (author_emp_id) REFERENCES Employee(emp_id)
);
CREATE TABLE SuggestionTimestamp(
    timestamp_id int NOT NULL PRIMARY KEY AUTO_INCREMENT,
    suggestion_id int NOT NULL,
    createdTimestamp datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
    planTimestamp datetime,
    doTimestamp datetime,
    studyTimestamp datetime,
    actTimestamp datetime,
    lastUpdatedTimestamp datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
    dueByTimestamp datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT SuggestionTimestampFK FOREIGN KEY (suggestion_id) REFERENCES Suggestion(suggestion_id)
);
CREATE TABLE SuggestionComment(
    comment_id int NOT NULL PRIMARY KEY AUTO_INCREMENT,
    emp_id int NOT NULL,
    suggestion_id int NOT NULL,
    description nvarchar(6000) NOT NULL,
    createdTimestamp datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT EmployeeCommentFK FOREIGN KEY (emp_id) REFERENCES Employee(emp_id),
    CONSTRAINT SuggestionFK FOREIGN KEY (suggestion_id) REFERENCES Suggestion(suggestion_id)
);
CREATE TABLE Category(
    category_id int NOT NULL PRIMARY KEY AUTO_INCREMENT,
    category_name varchar(100) NOT NULL
);
CREATE TABLE SuggestionCategory(
    suggestion_id int NOT NULL,
    category_id int NOT NULL,
    CONSTRAINT SuggestionCategoryFK FOREIGN KEY (suggestion_id) REFERENCES Suggestion(suggestion_id),
    CONSTRAINT CategoryFK FOREIGN KEY (category_id) REFERENCES Category(category_id),
    CONSTRAINT SuggestionCategoryPK PRIMARY KEY (suggestion_id, category_id)
);
ALTER TABLE Suggestion CONVERT TO CHARACTER SET utf8 COLLATE utf8_danish_ci;
/*GENERER ROLLER*/
INSERT INTO Role(role_name)
VALUES('Dørsliper');
INSERT INTO Role(role_name)
VALUES('Hengselfester');
INSERT INTO Role(role_name)
VALUES('Håndtakinspektor');
INSERT INTO Role(role_name)
VALUES('Karmvurderer');
INSERT INTO Role(role_name)
VALUES('Maleindivid');
INSERT INTO Role(role_name)
VALUES('Vindu innlemmer');
INSERT INTO Role(role_name)
VALUES('Dordesigner');
INSERT INTO Role(role_name)
VALUES('Malemester');
INSERT INTO Role(role_name)
VALUES('IT person');
INSERT INTO Role(role_name)
VALUES('Statistikkfører');
/*GENERER ROLLER TIL AUTENTISERING*/
INSERT INTO AuthorizationRole(authorization_role_name)
VALUES ("User");
INSERT INTO AuthorizationRole(authorization_role_name)
VALUES ("Admin");
INSERT INTO AuthorizationRole(authorization_role_name)
VALUES ("TeamLead");
/*GENERER ANSATTE*/
INSERT INTO Employee(
        emp_id,
        name,
        passwordhash,
        salt,
        role_id,
        authorization_role_id
    )
VALUES (1, "Arne Knutson", "password", "hei", 1, 1);
INSERT INTO Employee(
        emp_id,
        name,
        passwordhash,
        salt,
        role_id,
        authorization_role_id
    )
VALUES (2, "Carl Thorsen", "password1", "hei", 2, 2);
INSERT INTO Employee(
        emp_id,
        name,
        passwordhash,
        salt,
        role_id,
        authorization_role_id
    )
VALUES (3, "Bendik Borresen", "password2", "hei", 3, 1);
INSERT INTO Employee(
        emp_id,
        name,
        passwordhash,
        salt,
        role_id,
        authorization_role_id
    )
VALUES (4, "Sara Larsen", "password3", "hei", 4, 1);
INSERT INTO Employee(
        emp_id,
        name,
        passwordhash,
        salt,
        role_id,
        authorization_role_id
    )
VALUES(5, "Brage Martinson", "password4", "hei", 5, 1);
INSERT INTO Employee(
        emp_id,
        name,
        passwordhash,
        salt,
        role_id,
        authorization_role_id
    )
VALUES (6, "Jenny Smith", "password5", "hei", 6, 1);
INSERT INTO Employee(
        emp_id,
        name,
        passwordhash,
        salt,
        role_id,
        authorization_role_id
    )
VALUES (7, "Marie Hanson", "password6", "hei", 7, 1);
INSERT INTO Employee(
        emp_id,
        name,
        passwordhash,
        salt,
        role_id,
        authorization_role_id
    )
VALUES (8, "Nelle Hoftstadter", "password7", "hei", 8, 1);
INSERT INTO Employee(
        emp_id,
        name,
        passwordhash,
        salt,
        role_id,
        authorization_role_id
    )
VALUES (9, "Ole Willam Erikson", "password8", "hei", 9, 1);
INSERT INTO Employee(
        emp_id,
        name,
        passwordhash,
        salt,
        role_id,
        authorization_role_id
    )
VALUES (10, "Vanessa Merkel", "password9", "hei", 10, 1);
/*GENERER TEAMS*/
INSERT INTO Team(team_name, team_lead_id)
VALUES ('Ledergruppe', 1);
INSERT INTO Team(team_name, team_lead_id)
VALUES ('Salg og marked', 2);
INSERT INTO Team(team_name, team_lead_id)
VALUES ('Produksjon', 3);
INSERT INTO Team(team_name, team_lead_id)
VALUES ('Teknisk', 4);
INSERT INTO Team(team_name, team_lead_id)
VALUES ('Logistikk', 1);
INSERT INTO Team(team_name, team_lead_id)
VALUES ('Kundeserivce', 1);
INSERT INTO Team(team_name, team_lead_id)
VALUES ('Klageavdeling', 1);
INSERT INTO Team(team_name, team_lead_id)
VALUES ('Dørhåndtakavdeling', 1);
INSERT INTO Team(team_name, team_lead_id)
VALUES ('Listavdeling', 1);
INSERT INTO Team(team_name, team_lead_id)
VALUES ('Hengselavdeling', 1);
/*GENERER TEAMLIST*/
INSERT INTO TeamList(emp_id, team_id)
VALUES(1, 1);
INSERT INTO TeamList(emp_id, team_id)
VALUES(1, 2);
INSERT INTO TeamList(emp_id, team_id)
VALUES(2, 3);
INSERT INTO TeamList(emp_id, team_id)
VALUES(3, 2);
INSERT INTO TeamList(emp_id, team_id)
VALUES(4, 1);
INSERT INTO TeamList(emp_id, team_id)
VALUES(5, 1);
INSERT INTO TeamList(emp_id, team_id)
VALUES(6, 1);
INSERT INTO TeamList(emp_id, team_id)
VALUES(7, 1);
INSERT INTO TeamList(emp_id, team_id)
VALUES(8, 1);
INSERT INTO TeamList(emp_id, team_id)
VALUES(9, 1);
INSERT INTO TeamList(emp_id, team_id)
VALUES(10, 1);
INSERT INTO TeamList(emp_id, team_id)
VALUES(2, 9);
INSERT INTO TeamList(emp_id, team_id)
VALUES(3, 4);
/*GENERER KATEGORIER*/
INSERT INTO Category(category_name)
VALUES ("HMS");
INSERT INTO Category(category_name)
VALUES ("Kvalitet");
INSERT INTO Category(category_name)
VALUES ("Ledetid");
INSERT INTO Category(category_name)
VALUES ("Kostnader");
INSERT INTO Category(category_name)
VALUES ("Effektivisering");
INSERT INTO Category(category_name)
VALUES ("Kompetanse");
INSERT INTO Category(category_name)
VALUES ("Kommunikasjon");
INSERT INTO Category(category_name)
VALUES ("5S");
INSERT INTO Category(category_name)
VALUES ("Standardisering");
INSERT INTO Category(category_name)
VALUES ("Flyt");
INSERT INTO Category(category_name)
VALUES ("Visualisering");
INSERT INTO Category(category_name)
VALUES ("Energi");
INSERT INTO Category(category_name)
VALUES ("Bærekraft");
INSERT INTO Category(category_name)
VALUES ("Industri 4.0");
/*GENERER FORSLAG*/
INSERT INTO Suggestion(
        title,
        description,
        status,
        ownership_emp_id,
        author_emp_id
    )
VALUES (
        "Egen stand",
        "Vi burde ha egen stand for destillert vann der truckene står for lading. Nå er det litt kaos med vannet og ofte finner vi det ikke når vi behøver det.",
        "JustDoIt",
        2,
        2
    );
INSERT INTO Suggestion(
        title,
        description,
        status,
        ownership_emp_id,
        author_emp_id
    )
VALUES (
        "Flytte installasjonsbordet",
        "Installasjonsbordet som står ved EL-skapet bør flyttes til den andre siden av EL-skapet. Det er vanskelig å få tilgang til EL-skapet sånn som bordet står nå.",
        "Plan",
        3,
        3
    );
INSERT INTO Suggestion(
        title,
        description,
        status,
        ownership_emp_id,
        author_emp_id
    )
VALUES (
        "Flytte euro",
        "Europallene ble flyttet ut til høyre for inngangen til Hall A og ikke inne. De stod i veien.",
        "Act",
        4,
        4
    );
INSERT INTO Suggestion(
        title,
        description,
        status,
        ownership_emp_id,
        author_emp_id
    )
VALUES (
        "Ingen røyking!",
        "Askebegeret må flyttes fra inngangen til Hall A til andre siden av parkeringsplassen under eiketreet. Ingen røyking på arbeidsplassen!",
        "Study",
        5,
        4
    );
INSERT INTO Suggestion(
        title,
        description,
        status,
        ownership_emp_id,
        author_emp_id
    )
VALUES (
        "Heve/senke sengene",
        "Kan vi få mulighet til å heve å senke sengene? De sliter litt på kroppen når de ikke er grei høyde.",
        "Do",
        8,
        1
    );
INSERT INTO Suggestion(
        title,
        description,
        status,
        ownership_emp_id,
        author_emp_id
    )
VALUES (
        "Vi vil ha kjøttbollene tilbake!",
        "Kan vi får tilbake kjøttbollene i kantina? Jeg er så lei av alt det vegandrittet. #reddkjøttbollene!",
        "Plan",
        9,
        9
    );
INSERT INTO Suggestion(
        title,
        description,
        status,
        ownership_emp_id,
        author_emp_id
    )
VALUES (
        "Ny mekanisk rampe",
        "Ny mekanisk rampe til Hall B. Den som vi har nå har sluttet å fungere for lenge siden og vi har stadig problemer når biler kommer for å tømmes eller lastes. Last blir skadet og kan derfor ikke brukes! Koster vel mindre enn å fikse med tanke på hvor herpa den er.",
        "Study",
        7,
        1
    );
INSERT INTO Suggestion(
        title,
        description,
        status,
        ownership_emp_id,
        author_emp_id
    )
VALUES (
        "Zoner i hall B",
        "Male eller teipe gulvet på nytt i Hall B så at zonene blir mer tydelige.",
        "Act",
        6,
        6
    );
INSERT INTO Suggestion(
        title,
        description,
        status,
        ownership_emp_id,
        author_emp_id
    )
VALUES (
        "Blomster i asfalten?",
        "Siden vi ikke gidder å asfaltere parkeringsplassen bak Hall B, kan vi da plante blomster i hullene i asfalten? Parkeringsplassen blir mer koselig da og hullene blir mer synlig så at vi ikke kjører ned i de.",
        "Plan",
        9,
        9
    );
INSERT INTO Suggestion(
        title,
        description,
        status,
        ownership_emp_id,
        author_emp_id
    )
VALUES (
        "Vinsjer i taket",
        "Installere vinsjer i taket for å flytte på det vi jobber med enklere og raskere enn trallene vi bruker nå",
        "Act",
        6,
        2
    );
/*GENERER TIMESTAMPS*/
INSERT INTO SuggestionTimestamp(suggestion_id)
VALUES(1);
INSERT INTO SuggestionTimestamp(suggestion_id)
VALUES(2);
INSERT INTO SuggestionTimestamp(suggestion_id)
VALUES(3);
INSERT INTO SuggestionTimestamp(suggestion_id)
VALUES(4);
INSERT INTO SuggestionTimestamp(suggestion_id)
VALUES(5);
INSERT INTO SuggestionTimestamp(suggestion_id)
VALUES(6);
INSERT INTO SuggestionTimestamp(suggestion_id)
VALUES(7);
INSERT INTO SuggestionTimestamp(suggestion_id)
VALUES(8);
INSERT INTO SuggestionTimestamp(suggestion_id)
VALUES(9);
INSERT INTO SuggestionTimestamp(suggestion_id)
VALUES(10);
/*GENERER SUGGESTIONCATEGORY*/
INSERT INTO SuggestionCategory(suggestion_id, category_id)
VALUES(1, 3);
INSERT INTO SuggestionCategory(suggestion_id, category_id)
VALUES(2, 3);
INSERT INTO SuggestionCategory(suggestion_id, category_id)
VALUES(3, 3);
INSERT INTO SuggestionCategory(suggestion_id, category_id)
VALUES(4, 3);
INSERT INTO SuggestionCategory(suggestion_id, category_id)
VALUES(5, 3);
INSERT INTO SuggestionCategory(suggestion_id, category_id)
VALUES(6, 3);
INSERT INTO SuggestionCategory(suggestion_id, category_id)
VALUES(7, 3);
INSERT INTO SuggestionCategory(suggestion_id, category_id)
VALUES(8, 3);
INSERT INTO SuggestionCategory(suggestion_id, category_id)
VALUES(9, 3);
INSERT INTO SuggestionCategory(suggestion_id, category_id)
VALUES(10, 3);
INSERT INTO SuggestionCategory(suggestion_id, category_id)
VALUES(1, 2);
INSERT INTO SuggestionCategory(suggestion_id, category_id)
VALUES(3, 9);
INSERT INTO SuggestionCategory(suggestion_id, category_id)
VALUES(5, 2);
/*GENERER KOMMENTARER*/
INSERT INTO SuggestionComment(emp_id, suggestion_id, description)
VALUES (
        1,
        1,
        "Tenker dere burde oppdatere utstyret for bedre gjennomføring."
    );
INSERT INTO SuggestionComment(emp_id, suggestion_id, description)
VALUES (
        2,
        2,
        "Prøv å få bedre oversikt over systemet og se etter feilmeldinger."
    );
INSERT INTO SuggestionComment(emp_id, suggestion_id, description)
VALUES (
        3,
        3,
        "Se etter inspirasjon på nettet. Noen linker er fine til dette. Link: https://tester.com/%22"
    );
INSERT INTO SuggestionComment(emp_id, suggestion_id, description)
VALUES (
        4,
        4,
        "Burde kjøpe ny maskin, det øker effektiviteten."
    );
INSERT INTO SuggestionComment(emp_id, suggestion_id, description)
VALUES (
        5,
        5,
        "Planlegg møtene bedre gjennom Microft teams, da kan dere også legge til filer."
    );
INSERT INTO SuggestionComment(emp_id, suggestion_id, description)
VALUES (
        6,
        6,
        "Spør <Team> om hjelp, de har erfaring med dette."
    );
INSERT INTO SuggestionComment(emp_id, suggestion_id, description)
VALUES (
        7,
        7,
        "Det er bare noen småendringer og sjekk etter skrivefeil"
    );
INSERT INTO SuggestionComment(emp_id, suggestion_id, description)
VALUES (
        8,
        8,
        "PDF-en i forslaget burde ha samme format over alt."
    );
INSERT INTO SuggestionComment(emp_id, suggestion_id, description)
VALUES (
        9,
        9,
        "Dere burde få mer informasjon fra kundene!"
    );
INSERT INTO SuggestionComment(emp_id, suggestion_id, description)
VALUES (10, 10, "Sjekk dette. Link: https://test.com/%22");