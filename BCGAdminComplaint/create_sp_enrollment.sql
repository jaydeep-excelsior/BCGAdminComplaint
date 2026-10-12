CREATE PROCEDURE [dbo].[SP_GetEnrollmentDetails_ByEnroNumber]
    @EnroNumber NVARCHAR(250)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 
        e.EId AS EnrollmentId,
        e.FullName, 
        e.ContactNumber, 
        e.EmailAddress, 
        b.Description AS BarAssociation
    FROM tbl_Enrollment_Mst e 
    LEFT JOIN tbl_bar_association_mst b ON e.BarAssoId = b.Id 
    WHERE e.EnroNumber = @EnroNumber
END
