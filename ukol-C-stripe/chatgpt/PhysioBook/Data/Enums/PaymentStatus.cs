namespace PhysioBook.Data.Enums;

/// <summary>
/// Stav platby rezervace.
/// </summary>
public enum PaymentStatus
{
    /// Rezervace ještě nebyla zaplacena.
    Unpaid,

    /// Rezervace byla úspěšně zaplacena.
    Paid,

    /// Checkout session vypršela, rezervace je zrušena.
    Expired
}
