﻿namespace DennerBanx.Communication.Requests;
public class RequestEventJson
{
    public string? Type { get; set; }
    public string? Origin { get; set; }
    public string? Destination { get; set; }
    public decimal Amount { get; set; }

}
