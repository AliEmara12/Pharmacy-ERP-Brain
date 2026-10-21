namespace PharmacyERP.Domain.Enums;

public enum InventoryTransactionType
{
	OPENING_BALANCE = 1,
	ADJUSTMENT_IN = 2,
	ADJUSTMENT_OUT = 3,
	SALE_ISSUE = 4,
	SALES_RETURN_IN = 5,
	PURCHASE_RECEIPT = 6,
	VENDOR_RETURN_OUT = 7,
	SCRAP_OUT = 8,
	REVERSAL = 9
}