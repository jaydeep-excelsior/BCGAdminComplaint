$content = Get-Content -Path 'd:\Jaydeep\BCGAdminComplaint\BCGAdminComplaint\Views\ComplaintDetails\DetailsNew.cshtml' -Raw
$regex = '(?s)function updateActionButtonsUI.*?}\s*function updateStatusDirectly.*?}\s*}\s*function updateStatusWithNote.*?}\s*}\s*function triggerBoardHasApproved.*?\n    }'
if ($content -match $regex) {
    Set-Content 'extracted_functions.js' $matches[0]
} else {
    Write-Host "No match"
}
