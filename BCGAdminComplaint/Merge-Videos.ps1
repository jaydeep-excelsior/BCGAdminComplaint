param (
    [string]$VideoFormat = "*.mp4",
    [string]$OutputFileName = "merged_output.mp4"
)

# Check if ffmpeg is installed
if (!(Get-Command ffmpeg -ErrorAction SilentlyContinue)) {
    Write-Host "FFmpeg is not installed or not in your PATH." -ForegroundColor Red
    Write-Host "Please install FFmpeg (e.g., using 'winget install ffmpeg') before running this script." -ForegroundColor Yellow
    exit
}

$videos = Get-ChildItem -Filter $VideoFormat | Sort-Object Name

if ($videos.Count -eq 0) {
    Write-Host "No videos found with format $VideoFormat in the current directory." -ForegroundColor Yellow
    exit
}

Write-Host "Found $($videos.Count) video(s). Generating list..." -ForegroundColor Cyan

$listFile = "mylist.txt"
if (Test-Path $listFile) {
    Remove-Item $listFile
}

foreach ($vid in $videos) {
    Add-Content -Path $listFile -Value "file '$($vid.Name)'"
}

Write-Host "List generated. Starting merge process..." -ForegroundColor Cyan

# Run ffmpeg to merge
ffmpeg -f concat -safe 0 -i $listFile -c copy $OutputFileName

if ($LASTEXITCODE -eq 0) {
    Write-Host "Successfully merged videos into $OutputFileName!" -ForegroundColor Green
    # Clean up the temporary list file
    Remove-Item $listFile
} else {
    Write-Host "An error occurred during the merge process." -ForegroundColor Red
}
