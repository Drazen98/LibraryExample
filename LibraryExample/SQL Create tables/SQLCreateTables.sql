
/*
CREATE TABLE AutorKnjige(
    ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    ime_autora varchar(50),
	godina_rodjenja int
);

CREATE TABLE Zanrovi (
    ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    ime_zanra varchar(50)
);

CREATE TABLE Knjige(
    ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    ime_knjige varchar(50),
	autorKnjigeId INT NOT NULL,
	zanrId INT NOT NULL,
	CONSTRAINT fk_autorKnjigeId FOREIGN KEY(autorKnjigeId) REFERENCES AutorKnjige(ID)
	ON DELETE CASCADE,
	CONSTRAINT fk_zanrId FOREIGN KEY(zanrID) REFERENCES Zanrovi(ID)
	ON DELETE CASCADE,
	datum_unosa datetime default CURRENT_TIMESTAMP
);
*/