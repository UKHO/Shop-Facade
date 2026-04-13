Param(
	[Parameter(mandatory=$true)][string]$healthEndPointUrl,
    [Parameter(mandatory=$true)][string]$waitTimeInMinute
)

$sleepTimeInSecond = 15
$isServiceActive = 'false'

$stopWatch = New-Object -TypeName System.Diagnostics.Stopwatch
$timeSpan = New-TimeSpan -Minutes $waitTimeInMinute
$stopWatch.Start()

do
{
    Write-Host "Polling url: $healthEndPointUrl ..."
    try{
        $HttpRequest  = [System.Net.WebRequest]::Create("$healthEndPointUrl")
        $HttpResponse = $HttpRequest.GetResponse()
        $HttpStatus   = $HttpResponse.StatusCode
        Write-Host "Status code of web is $HttpStatus ..."
    
        If ($HttpStatus -eq 200 ) {
            Write-Host "Service is up. Stopping Polling ..."
            $isServiceActive = 'true'
            break
        }
        Else {
            Write-Host "Service not yet Up. Status code: $HttpStatus re-checking after $sleepTimeInSecond sec ..."
        }
    }
    catch
    {
        $HttpStatus = $null
        $errorMessage = $_.Exception.Message
        
        if ($_.Exception -is [System.Net.WebException] -and $null -ne $_.Exception.Response) {
            $HttpStatus = [int]$_.Exception.Response.StatusCode
            Write-Host "Service not yet Up. Status: $HttpStatus ($($_.Exception.Response.StatusDescription)) re-checking after $sleepTimeInSecond sec ..."
        }
        else {
            Write-Host "Service not yet Up. Error: $errorMessage - re-checking after $sleepTimeInSecond sec ..."
        }
    }    
    
    Start-Sleep -Seconds $sleepTimeInSecond
}
until ($stopWatch.Elapsed -ge $timeSpan)


If ($null -ne $HttpResponse) { 
    $HttpResponse.Close() 
}

if ($isServiceActive -eq 'true' ) {
    Write-Host "Service is up returning from script ..."
}
Else { 
    Write-Error "Service was not up in $waitTimeInMinute minutes, error while deployment ..."
    throw "Error"
}
