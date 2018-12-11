To use the application and add a vendor:

	Prerequisites:
		InitScript.sql and AddTestData.sql have been executed
		Either VisualStudio or the MRPApp.exe have been started in administrator mode.

	Upon launch you with be faced with a two-tabbed window.

	Info Tab:
		Fill in all text boxes with their appropriate information
	Products Tab:
		All of the products in the database are listed
		Click on a part to select it, click on it again to deselect
		Type in the fliter box to search for products
			NOTE: Parts remain selected even if they have been filtered out

	Click on the "Save" button when all information has been entered and at least one product has been selected
	
To test application ran correctly, run the following query
SELECT partID, partName, CompanyName
FROM Parts
JOIN Vendor ON Parts.supplyVendorId = Vendor.VendorId