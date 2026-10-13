ALTER PROCEDURE [dbo].[SP_DCMember_Save]
    @Id BIGINT,
    @EnrollmentId BIGINT,
    @EmailAddress NVARCHAR(MAX),
    @Password NVARCHAR(MAX),
    @Flag NCHAR(1),
    @CreateUser BIGINT,
    @ActionMode NVARCHAR(50),
    @Name NVARCHAR(250) = NULL,
    @ContactNo NVARCHAR(50) = NULL,
    @BarAssociation NVARCHAR(500) = NULL,
    @MemberType NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- ========== INSERT RECORD ==========
    IF @ActionMode = 'InsertRecord'
    BEGIN
        IF EXISTS (SELECT 1 FROM tbl_Member WHERE EnrollmentId = @EnrollmentId AND Flag = 'A')
        BEGIN
            SELECT 'EX' AS ReturnStatus, '0' AS Id
            RETURN
        END

        INSERT INTO tbl_Member (EnrollmentId, EmailAddress, [Password], Flag, CreateUser, CreateDate, MemberType)
        VALUES (@EnrollmentId, @EmailAddress, @Password, @Flag, @CreateUser, GETDATE(), @MemberType)

        SELECT 'IN' AS ReturnStatus, CAST(SCOPE_IDENTITY() AS NVARCHAR(50)) AS Id
    END

    -- ========== UPDATE RECORD ==========
    ELSE IF @ActionMode = 'UpdateRecord'
    BEGIN
        IF EXISTS (SELECT 1 FROM tbl_Member WHERE EnrollmentId = @EnrollmentId AND Id != @Id AND Flag = 'A')
        BEGIN
            SELECT 'EX' AS ReturnStatus, '0' AS Id
            RETURN
        END

        UPDATE tbl_Member
        SET EnrollmentId = @EnrollmentId,
            EmailAddress = @EmailAddress,
            UpdateUser = @CreateUser,
            UpdateDate = GETDATE(),
            MemberType = @MemberType
        WHERE Id = @Id

        SELECT 'UD' AS ReturnStatus, CAST(@Id AS NVARCHAR(50)) AS Id
    END

    -- ========== STATUS UPDATE (Activate/Inactivate) ==========
    ELSE IF @ActionMode = 'StatusUpdateRecord'
    BEGIN
        UPDATE tbl_Member
        SET Flag = @Flag,
            UpdateUser = @CreateUser,
            UpdateDate = GETDATE()
        WHERE Id = @Id

        SELECT 'UD' AS ReturnStatus, CAST(@Id AS NVARCHAR(50)) AS Id
    END
END
