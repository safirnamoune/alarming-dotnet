[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

function Write-Utf8 {
    param([string]$Path, [string]$Content)
    $full = [System.IO.Path]::GetFullPath((Join-Path (Get-Location) $Path))
    [System.IO.File]::WriteAllText($full, $Content, (New-Object System.Text.UTF8Encoding($false)))
}

function Get-WithRetry {
    param([string]$Uri, [string]$OutFile)
    $agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
    for ($essai = 1; $essai -le 5; $essai++) {
        try {
            if ($OutFile) {
                Invoke-WebRequest -Uri $Uri -UserAgent $agent -UseBasicParsing -TimeoutSec 60 -OutFile $OutFile
                return "ok"
            }
            return (Invoke-WebRequest -Uri $Uri -UserAgent $agent -UseBasicParsing -TimeoutSec 60).Content
        }
        catch {
            Write-Host ("  essai {0}/5 echoue : {1}" -f $essai, $_.Exception.Message)
            Start-Sleep -Seconds 4
        }
    }
    return $null
}

$url = "https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap"
$fontsDir = ".\public\fonts"
New-Item -ItemType Directory -Path $fontsDir -Force | Out-Null

Write-Host "Recuperation de la feuille de style Google..."
$feuille = Get-WithRetry -Uri $url

if (-not $feuille) {
    Write-Host ""
    Write-Host "ECHEC RESEAU : repli sur le chargement depuis Google a l affichage."
    $idx = Get-Content .\src\index.html -Raw
    if ($idx -notmatch "Jakarta") {
        $idx = $idx -replace "</head>", ('  <link rel="stylesheet" href="' + $url + '" />' + "`r`n</head>")
        Write-Utf8 -Path "src/index.html" -Content $idx
        Write-Host "index.html : lien Google restaure."
    }
    $ng = Get-Content .\angular.json -Raw | ConvertFrom-Json
    $ng.projects."alarming-web".architect.build.options.styles = @("src/theme.scss", "src/styles.css")
    Write-Utf8 -Path "angular.json" -Content ($ng | ConvertTo-Json -Depth 100)
    Remove-Item .\src\fonts.css -Force -ErrorAction SilentlyContinue
    Write-Host "Etat coherent retabli. A retenter plus tard."
    exit 1
}

$liens = [regex]::Matches($feuille, 'url\((https://fonts\.gstatic\.com/[^)]+\.woff2)\)')
Write-Host ("Fichiers de police a telecharger : {0}" -f $liens.Count)

foreach ($lien in $liens) {
    $adresse = $lien.Groups[1].Value
    $nom = Split-Path $adresse -Leaf
    if (-not (Get-WithRetry -Uri $adresse -OutFile (Join-Path $fontsDir $nom))) {
        Write-Host ("  MANQUANT : {0}" -f $nom)
    }
}

$locale = [regex]::Replace($feuille, 'https://fonts\.gstatic\.com/[^)]*/([^)/]+\.woff2)', '/fonts/$1')
Write-Utf8 -Path "src/fonts.css" -Content $locale

Write-Host ""
Write-Host ("Polices locales presentes : {0}" -f (Get-ChildItem $fontsDir -Filter *.woff2).Count)
Write-Host "Termine."