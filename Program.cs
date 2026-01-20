using CodingChallenges;


LazyExample.Run();

Console.Read();

var processor = new IdempotentReqProcessor();

PaymentResult result = await processor.ProcessPaymentAsync(
    idempotencyKey: "demo-123",
    processFunc: async () =>
    {
        await Task.Delay(50);
        return new PaymentResult(true, "OK");
    });

Console.WriteLine($"Success: {result.Success}, Message: {result.Message}");
