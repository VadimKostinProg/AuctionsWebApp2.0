INSERT INTO Users 
(
	FullName,
	DateOfBirth,
	IsEmailConfirmed,
	AverageScore,
	Status,
	CreatedAt,
	CreatedBy,
	Deleted,
	Username,
	Email,
	PasswordHashed,
	PasswordSalt,
	RoleId
)
VALUES (
	'Vadym Kostin', 
	'2003-09-26T00:00:00.0', 
	1, 
	0, 
	0, 
	GETUTCDATE(), 
	'system', 
	0, 
	'sysAdmin', 
	'vadkostinwork@gmail.com', 
	'd8he8TlpKZwQP1oOC2mICs9KKPv5bx3NcgQ3HtVxCWM=', 
	'Ky/21BIIi+PftirZNiqc0etWWNsnJUfVGCZ2o5Lsp2uXYHe5iQLNPAOwaCmj/8hFPG8Aft3qeFVUEplH4WE2AqCe8H5Mei0G9cTprHjPDvx7LWtK2+LF7wEZjrQSUOl9IGG4j4RG4RK51vlX7YzScEvafmShqlCQ2Dxe7faTx5U=', 
	1
)