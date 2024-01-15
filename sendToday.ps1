$today = Get-Date((get-date))  -Format "dd.MM.yyyy"
echo $today

$yesterday = Get-Date((get-date).AddDays(-1))  -Format "dd.MM.yyyy"
echo $yesterday

Invoke-WebRequest -Uri ('https://localhost:7224/api/Test/SendMatchesToChannel?date=' + $today)