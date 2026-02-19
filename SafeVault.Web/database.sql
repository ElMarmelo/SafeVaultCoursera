CREATE TABLE Users (
    UserID INT PRIMARY KEY AUTO_INCREMENT,
    Username VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
)

-- Stored Procedure as suggested by Copilot --
DELIMITER $$

CREATE PROCEDURE GetUserByUsername(IN p_Username VARCHAR(100))
BEGIN
    SELECT UserID, Username, Email FROM Users WHERE Username = p_Username;
END$$

DELIMITER ;
