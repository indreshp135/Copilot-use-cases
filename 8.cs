class ApiService
{

    private static readonly HttpClient _client = new HttpClient();

    public async Task<string> FetchDataFromApiWithRetryAsync(string apiUrl, CancellationToken cancellationToken, int maxRetries, int cancellationDelay, Label statusLabel)
    {
        var retryCount = 0;
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        while (retryCount < maxRetries)
        {
            try
            {
                cts.CancelAfter(cancellationDelay);
                return await FetchDataFromApiAsync(cts.Token, statusLabel);
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                if (retryCount < maxRetries - 1)
                {
                    retryCount++;
                    int delay = (int)Math.Pow(2, retryCount) * 1000;
                    await Task.Delay(delay, cancellationToken);
                    UpdateStatusLabel($"Retrying ({retryCount}/{maxRetries})...", statusLabel);
                }
                else
                {
                    throw new Exception($"Failed to fetch data after {maxRetries} retries: {ex.Message}", ex);
                }
            }
        }

        throw new OperationCanceledException("The operation was cancelled.", cts.Token);
    }

    private async Task<string> FetchDataFromApiAsync(string apiUrl, CancellationToken cancellationToken, Label statusLabel)
    {
        UpdateStatusLabel("Fetching data...", statusLabel);

        HttpResponseMessage response = await _client.GetAsync(apiUrl, cancellationToken);
        response.EnsureSuccessStatusCode();
        string data = await response.Content.ReadAsStringAsync();

        UpdateStatusLabel("Data fetched.", statusLabel);
        return data;
    }

    private void UpdateStatusLabel(string message, Label statusLabel)
    {
        if (statusLabel.InvokeRequired)
        {
            statusLabel.BeginInvoke(new Action<string, Label>(UpdateStatusLabel), message, statusLabel);
        }
        else
        {
            statusLabel.Text = message;
        }
    }
}