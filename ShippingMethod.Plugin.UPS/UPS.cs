﻿using ShippingMethod.Plugin.UPS.Domain;
using ShippingMethod.Plugin.UPS.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ShippingMethod.Plugin.UPS
{
    public class UPS
    {
        #region Properties

        public string AccessNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ShipperNumber { get; set; }
        public string Uri { get; set; } = "https://www.ups.com/ups.app/xml/Rate?";
        public string Code { get; set; }
        public string PickUpType { get; set; }
        #endregion

        #region Classes
        public UPS()
        {
        }

        public UPS(string accessNumber, string userName, string password, string shipperNumber, string pickupType)
        {
            AccessNumber = accessNumber;
            UserName = userName;
            Password = password;
            ShipperNumber = shipperNumber;
            PickUpType = pickupType;
        }

        #endregion

        #region Methods

        public void SetCredentials(string accessNumber, string userName, string password, string shipperNumber)
        {
            AccessNumber = accessNumber;
            UserName = userName;
            Password = password;
            ShipperNumber = shipperNumber;
        }

        #endregion

        #region Rate
        public string RatingServiceSelectionRequest(Address shipperAddress, Address destinationAddress, List<Package> packages, string serviceCode, string pickUpType)
        {
            String request = "<?xml version=\"1.0\"?>\n" +
                    "    \t<AccessRequest xml:lang=\"en-US\">\n" +
                    "    \t\t<AccessLicenseNumber>" + AccessNumber + "</AccessLicenseNumber>\n" +
                    "    \t\t<UserId>" + UserName + "</UserId>\n" +
                    "    \t\t<Password>" + Password + "</Password>\n" +
                    "    \t</AccessRequest>\n" +
                    "    \t<?xml version=\"1.0\"?>\n" +
                    "    \t<RatingServiceSelectionRequest xml:lang=\"en-US\">\n" +
                    "    \t\t<Request>\n" +
                    "    \t\t\t<TransactionReference>\n" +
                    "    \t\t\t\t<CustomerContext>Bare Bones Rate Request</CustomerContext>\n" +
                    "    \t\t\t\t<XpciVersion>1.0001</XpciVersion>\n" +
                    "    \t\t\t</TransactionReference>\n" +
                    "    \t\t\t<RequestAction>Rate</RequestAction>\n" +
                    "    \t\t\t<RequestOption>Rate</RequestOption>\n" +
                    "    \t\t</Request>\n" +
                    "    \t<PickupType>\n" +
                    "    \t\t<Code>01</Code>\n" +
                    "                <Description>Regular Daily Pickup</Description>\n" +
                    "    \t</PickupType>\n" +
                    "    \t<Shipment>\n" +
                    "    \t<RateInformation>\n" +
                    "    \t\t<NegotiatedRatesIndicator/>\n" +
                    "    \t</RateInformation>\n" +
                    "    \t\t<Shipper>\n" +
                    "    \t\t\t<Address>\n" +
                    "    \t\t\t\t<PostalCode>" + shipperAddress.ZipCode + "</PostalCode>\n" +
                    "    \t\t\t\t<StateProvinceCode>" + shipperAddress.State.StateCode + "</StateProvinceCode>\n" +
                    "    \t\t\t\t<CountryCode>" + shipperAddress.Country.CountryCode + "</CountryCode>\n" +
                    "    \t\t\t</Address>\n" +
                    "\t\t\t<ShipperNumber>" + ShipperNumber + "</ShipperNumber>\n" +
                    "    \t\t</Shipper>\n" +
                    "    \t\t<ShipTo>\n" +
                    "    \t\t\t<Address>\n" +
                    "    \t\t\t\t<PostalCode>" + destinationAddress.ZipCode + "</PostalCode>\n" +
                    "    \t\t\t\t<CountryCode>" + destinationAddress.Country.CountryCode + "</CountryCode>\n" +
                    "\t\t\t\t<ResidentialAddressIndicator/>\n" +
                    "    \t\t\t</Address>\n" +
                    "    \t\t</ShipTo>\n" +
                    "    \t\t<ShipFrom>\n" +
                    "    \t\t\t<Address>\n" +
                    "    \t\t\t\t<City>" + shipperAddress.City + "</City>\n" +
                    "    \t\t\t\t<StateProvinceCode>" + shipperAddress.State.StateCode + "</StateProvinceCode>\n" +
                    "    \t\t\t\t<PostalCode>" + shipperAddress.ZipCode + "</PostalCode>\n" +
                    "    \t\t\t\t<CountryCode>US</CountryCode>\n" +
                    "    \t\t\t</Address>\n" +
                    "    \t\t</ShipFrom>\n" +
                    "    \t\t<Service>\n" +
                    "    \t\t\t<Code>" + serviceCode + "</Code>\n" +
                    "    \t\t</Service>\n";

                    for (var i = 0; i < packages.Count; i++)
                    {
                        request +=  "\t\t<Package>\n" +
                                    "\t\t\t<PackagingType>\n" +
                                    "\t\t\t\t<Code>02</Code>\n" +
                                    "\t\t\t</PackagingType>\n"+
                                    "\t\t\t<Dimensions>\n" +
                                    "\t\t\t\t<UnitOfMeasurement>\n" +
                                    "\t\t\t\t\t<Code>IN</Code>\n" +
                                    "\t\t\t\t</UnitOfMeasurement>\n" +
                                    "\t\t\t\t<Length>" + packages[i].RoundedLength.ToString() + "</Length>\n" +
                                    "\t\t\t\t<Width>" + packages[i].RoundedWidth.ToString() + "</Width>\n" +
                                    "\t\t\t\t<Height>" + packages[i].RoundedHeight.ToString() + "</Height>\n" +
                                    "\t\t\t</Dimensions>\n"+
                                    "\t\t\t<PackageWeight>\n" +
                                    "\t\t\t\t<UnitOfMeasurement>\n" +
                                    "\t\t\t\t\t<Code>LBS</Code>\n" +
                                    "\t\t\t\t</UnitOfMeasurement>\n" +
                                    "\t\t\t\t<Weight>" + packages[i].RoundedWeight.ToString() + "</Weight>\n" +
                                    "\t\t\t</PackageWeight>\n"+
                                    "\t\t\t<PackageServiceOptions>\n" +
                                    "\t\t\t<InsuredValue>\n" +
                                    "\t\t\t\t<CurrencyCode>USD</CurrencyCode>\n"+
                                    "\t\t\t\t<MonetaryValue>" + packages[i].InsuredValue.ToString()+"</MonetaryValue>\n"+
                                    "\t\t\t</InsuredValue>\n";
                        if (packages[i].SignatureRequiredOnDelivery)
                        {
                            request +="\t\t\t\t<DeliveryConfirmation>\n"+
                                        "\t\t\t\t<DCISType>2</DCISType>\n"+
                                        "\t\t\t\t</DeliveryConfirmation>\n";
                        }
                         request += "\t\t\t</PackageServiceOptions>\n"+
                                   "\t\t</Package>\n";
                    }
                    request += "\t</Shipment>\n" +
                               "</RatingServiceSelectionRequest>";

            return request;
        }

        public static IEnumerable<Shipment> GetRate(string response, bool saturdayDelivery, ref string error)
        {
            var shipmentList = new List<Shipment>();


            using (var sr = new StringReader(response))
            using (var tr = new XmlTextReader(sr))
                while (tr.Read())
                {
                    if ((tr.Name == "Error") && (tr.NodeType == XmlNodeType.Element))
                    {
                        var errorText = "";
                        while (tr.Read())
                        {
                            if ((tr.Name == "ErrorCode") && (tr.NodeType == XmlNodeType.Element))
                            {
                                errorText += "UPS Rating Error, Error Code: " + tr.ReadString() + ", ";
                            }
                            if ((tr.Name == "ErrorDescription") && (tr.NodeType == XmlNodeType.Element))
                            {
                                errorText += "Error Desc: " + tr.ReadString();
                            }
                        }
                        error = "UPS Error returned: " + errorText;
                    }
                    if ((tr.Name == "RatedShipment") && (tr.NodeType == XmlNodeType.Element))
                    {
                        var serviceCode = "";
                        var monetaryValue = "";
                        while (tr.Read())
                        {
                            if ((tr.Name == "Service") && (tr.NodeType == XmlNodeType.Element))
                            {
                                while (tr.Read())
                                {
                                    if ((tr.Name == "Code") && (tr.NodeType == XmlNodeType.Element))
                                    {
                                        serviceCode = tr.ReadString();
                                        tr.ReadEndElement();
                                    }
                                    if ((tr.Name == "Service") && (tr.NodeType == XmlNodeType.EndElement))
                                    {
                                        break;
                                    }
                                }
                            }
                            if ((tr.Name == "RatedShipment") && (tr.NodeType == XmlNodeType.EndElement))
                            {
                                break;
                            }
                            if ((tr.Name == "RatedPackage") && (tr.NodeType == XmlNodeType.Element))
                            {
                                while (tr.Read())
                                {
                                    if ((tr.Name == "RatedPackage") && (tr.NodeType == XmlNodeType.EndElement))
                                        break;
                                }
                            }
                            if ((tr.Name == "TotalCharges") && (tr.NodeType == XmlNodeType.Element))
                            {
                                while (tr.Read())
                                {
                                    if ((tr.Name == "MonetaryValue") && (tr.NodeType == XmlNodeType.Element))
                                    {
                                        monetaryValue = tr.ReadString();
                                        tr.ReadEndElement();
                                    }
                                    if ((tr.Name == "TotalCharges") && (tr.NodeType == XmlNodeType.EndElement))
                                    {
                                        break;
                                    }
                                }
                            }
                            //parse negotiated rates
                            if ((tr.Name == "NegotiatedRates") && (tr.NodeType == XmlNodeType.Element))
                            {
                                while (tr.Read())
                                {
                                    if ((tr.Name == "NetSummaryCharges") && (tr.NodeType == XmlNodeType.Element))
                                    {
                                        while (tr.Read())
                                        {
                                            if ((tr.Name == "GrandTotal") && (tr.NodeType == XmlNodeType.Element))
                                            {
                                                while (tr.Read())
                                                {
                                                    if ((tr.Name == "MonetaryValue") && (tr.NodeType == XmlNodeType.Element))
                                                    {
                                                        monetaryValue = tr.ReadString();
                                                        tr.ReadEndElement();
                                                    }
                                                    if ((tr.Name == "GrandTotal") && (tr.NodeType == XmlNodeType.EndElement))
                                                        break;
                                                }
                                            }
                                            if ((tr.Name == "NetSummaryCharges") && (tr.NodeType == XmlNodeType.EndElement))
                                                break;
                                        }
                                    }
                                    if ((tr.Name == "NegotiatedRates") && (tr.NodeType == XmlNodeType.EndElement))
                                        break;
                                }
                            }
                        }
                        var service = GetServiceName(serviceCode);
                        var serviceId = $"[{serviceCode}]";

                        // Go to the next rate if the service ID is not in the list of services to offer
                        if (!saturdayDelivery && string.IsNullOrEmpty(serviceId))
                        {
                            continue;
                        }

                        //Weed out unwanted or unknown service rates
                        if (service.ToUpper() != "UNKNOWN")
                        {
                            var shipment = new Shipment
                            {
                                Rate = Convert.ToDecimal(monetaryValue, new CultureInfo("en-US")),
                                Name = service
                            };
                            shipmentList.Add(shipment);
                        }

                    }
                }

            return shipmentList;
        }

        public string ShipmentConfirmRequest()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version='1.0'?>");
            sb.Append("	<AccessRequest xml:lang='en-US'>");
            sb.Append("		<AccessLicenseNumber>").Append(AccessNumber).Append("</AccessLicenseNumber>");
            sb.Append("		<UserId>").Append(UserName).Append("</UserId>");
            sb.Append("		<Password>").Append(Password).Append("</Password>");
            sb.Append("	</AccessRequest>");
            sb.Append("<?xml version='1.0'?>");
            sb.Append("<ShipmentConfirmRequest xml:lang=\"en-US\">");
            sb.Append("  <Request>");
            sb.Append("    <TransactionReference>");
            sb.Append("      <CustomerContext>Customer Comment</CustomerContext>");
            sb.Append("      <XpciVersion/>");
            sb.Append("    </TransactionReference>");
            sb.Append("    <RequestAction>ShipConfirm</RequestAction>");
            sb.Append("    <RequestOption>validate</RequestOption>");
            sb.Append("  </Request>");
            sb.Append("  <LabelSpecification>");
            sb.Append("    <LabelPrintMethod>");
            sb.Append("      <Code>GIF</Code>");
            sb.Append("      <Description>gif file</Description>");
            sb.Append("    </LabelPrintMethod>");
            sb.Append("    <HTTPUserAgent>Mozilla/4.5</HTTPUserAgent>");
            sb.Append("    <LabelImageFormat>");
            sb.Append("      <Code>GIF</Code>");
            sb.Append("      <Description>gif</Description>");
            sb.Append("    </LabelImageFormat>");
            sb.Append("  </LabelSpecification>");
            sb.Append("  <Shipment>");
            sb.Append("   <RateInformation>");
            sb.Append("      <NegotiatedRatesIndicator/> ");
            sb.Append("    </RateInformation> ");
            sb.Append("	<Description/>");
            sb.Append("    <Shipper>");
            sb.Append("      <Name>Shipper Name</Name>");
            sb.Append("      <PhoneNumber>1234567890</PhoneNumber>");
            sb.Append("      <ShipperNumber>Ship Number</ShipperNumber>");
            sb.Append("	  <TaxIdentificationNumber>1234567877</TaxIdentificationNumber>");
            sb.Append("      <Address>");
            sb.Append("    	<AddressLine1>Address Line</AddressLine1>");
            sb.Append("    	<City>City</City>");
            sb.Append("    	<StateProvinceCode>MD</StateProvinceCode>");
            sb.Append("    	<PostalCode>21093</PostalCode>");
            sb.Append("    	<PostcodeExtendedLow></PostcodeExtendedLow>");
            sb.Append("    	<CountryCode>US</CountryCode>");
            sb.Append("     </Address>");
            sb.Append("    </Shipper>");
            sb.Append("	<ShipTo>");
            sb.Append("     <CompanyName>Ship To Company Name</CompanyName>");
            sb.Append("      <AttentionName>Ship To Attn Name</AttentionName>");
            sb.Append("      <PhoneNumber>97225377171</PhoneNumber>");
            sb.Append("      <Address>");
            sb.Append("        <AddressLine1>Address Line</AddressLine1>");
            sb.Append("        <City>City</City>");
            sb.Append("        <StateProvinceCode>FL</StateProvinceCode>");
            sb.Append("        <PostalCode>32960</PostalCode>");
            sb.Append("        <CountryCode>US</CountryCode>");
            sb.Append("      </Address>");
            sb.Append("    </ShipTo>");
            sb.Append("    <ShipFrom>");
            sb.Append("      <CompanyName>Ship From Company Name</CompanyName>");
            sb.Append("      <AttentionName>Ship From Attn Name</AttentionName>");
            sb.Append("      <PhoneNumber>1234567890</PhoneNumber>");
            sb.Append("	  <TaxIdentificationNumber>1234567877</TaxIdentificationNumber>");
            sb.Append("      <Address>");
            sb.Append("        <AddressLine1>Address Line</AddressLine1>");
            sb.Append("        <City>City</City>");
            sb.Append("    	<StateProvinceCode>MD</StateProvinceCode>");
            sb.Append("    	<PostalCode>21093</PostalCode>");
            sb.Append("    	<CountryCode>US</CountryCode>");
            sb.Append("      </Address>");
            sb.Append("    </ShipFrom>");
            sb.Append("     <PaymentInformation>");
            sb.Append("      <Prepaid>");
            sb.Append("        <BillShipper>");
            sb.Append("          <AccountNumber>Ship Number</AccountNumber>");
            sb.Append("        </BillShipper>");
            sb.Append("      </Prepaid>");
            sb.Append("    </PaymentInformation>");
            sb.Append("    <Service>");
            sb.Append("      <Code>02</Code>");
            sb.Append("      <Description>2nd Day Air</Description>");
            sb.Append("    </Service>");
            sb.Append("    <Package>");
            sb.Append("      <PackagingType>");
            sb.Append("        <Code>02</Code>");
            sb.Append("        <Description>Customer Supplied</Description>");
            sb.Append("      </PackagingType>");
            sb.Append("      <Description>Package Description</Description>");
            sb.Append("	  <ReferenceNumber>");
            sb.Append("	  	<Code>00</Code>");
            sb.Append("		<Value>Package</Value>");
            sb.Append("	  </ReferenceNumber>");
            sb.Append("      <PackageWeight>");
            sb.Append("        <UnitOfMeasurement/>");
            sb.Append("        <Weight>60.0</Weight>");
            sb.Append("      </PackageWeight>");
            sb.Append("      <LargePackageIndicator/>");
            sb.Append("      <AdditionalHandling>0</AdditionalHandling>");
            sb.Append("    </Package>");
            sb.Append("  </Shipment>");
            sb.Append("</ShipmentConfirmRequest> ");

            return sb.ToString();
        }

        public string ShipmentAcceptRequest()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>");
            sb.Append("<AccessRequest>");
            sb.Append("    <AccessLicenseNumber>Your_License</AccessLicenseNumber>");
            sb.Append("    <UserId>Your_ID</UserId>");
            sb.Append("    <Password>Your_Password</Password>");
            sb.Append("</AccessRequest>");
            sb.Append("<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>");
            sb.Append("<ShipmentAcceptRequest>");
            sb.Append("    <Request>");
            sb.Append("        <TransactionReference>");
            sb.Append("            <CustomerContext>Customer Comment</CustomerContext>");
            sb.Append("        </TransactionReference>");
            sb.Append("        <RequestAction>ShipAccept</RequestAction>");
            sb.Append("        <RequestOption>1</RequestOption>");
            sb.Append("    </Request>");
            sb.Append("    <ShipmentDigest>rO0ABXNyACpjb20udXBzLmVjaXMuY29yZS5zaGlwbWVudHMuU2hpcG1lbnREaWdlc3SR92x3YMOrawIABloAF2lzQUJSUmV0dXJuZWRpblJlc3BvbnNlWgASbGFiZWxMaW5rSW5kaWNhdG9yTAAYSW50ZXJuYXRpb25hbEZvcm1zRGV0YWlsdAArTGNvbS91cHMvaWZjL2NvcmUvSW50ZXJuYXRpb25hbEZvcm1zRGV0YWlsO0wADGRpc3RyaWN0Q29kZXQAEkxqYXZhL2xhbmcvU3RyaW5nO0wADHNlY3VyaXR5Q29kZXEAfgACTAAIc2hpcG1lbnR0AB5MY29tL3Vwcy9jb21tb24vY29yZS9TaGlwbWVudDt4cAEAc3IAKWNvbS51cHMuaWZjLmNvcmUuSW50ZXJuYXRpb25hbEZvcm1zRGV0YWlsX8Nugzs/xBoCABVaABBwYXBlcmxlc3NJbnZvaWNlUwAHc2VkVHlwZVoAEHZhbGlkYXRlU2hpcG1lbnRMAAtSZXF1ZXN0VHlwZXEAfgACTAAMU2hpcG1lbnRJbmZvdAAfTGNvbS91cHMvaWZjL2NvcmUvU2hpcG1lbnRJbmZvO0wAEmNsaWVudE9wdGlvbmFsRGF0YXQAE0xqYXZhL3V0aWwvSGFzaE1hcDtMAB9jb21tZXJjaWFsSW52b2ljZUFkZGl0aW9uYWxJbmZvdAAyTGNvbS91cHMvaWZjL2NvcmUvQ29tbWVyY2lhbEludm9pY2VBZGRpdGlvbmFsSW5mbztMAAhjb250YWN0c3QAFUxqYXZhL3V0aWwvQXJyYXlMaXN0O0wADGN1cnJlbmN5Q29kZXEAfgACTAARZGF0ZU9mRXhwb3J0YXRpb25xAH4AAkwAEGV4cG9ydGluZ0NhcnJpZXJxAH4AAkwABWZvcm1zcQB+AAlMAAlmb3Jtc0Rlc2NxAH4AAkwADGZvcm1zR3JvdXBJRHEAfgACTAALZm9ybXNTdGF0dXNxAH4AAkwABmxvY2FsZXEAfgACTAAfbkFGVEFDZXJ0T2ZPcmlnaW5BZGRpdGlvbmFsSW5mb3QAOUxjb20vdXBzL2lmYy9jb3JlL05BRlRBQ2VydGlmaWNhdGVPZk9yaWdpbkFkZGl0aW9uYWxJbmZvO0wACHByb2R1Y3RzcQB+AAlMAAxyZXR1cm5Ub1BhZ2VxAH4AAkwAEXNFREFkZGl0aW9uYWxJbmZvdAAkTGNvbS91cHMvaWZjL2NvcmUvU0VEQWRkaXRpb25hbEluZm87TAAcdVNDZXJ0T2ZPcmlnaW5BZGRpdGlvbmFsSW5mb3QANkxjb20vdXBzL2lmYy9jb3JlL1VTQ2VydGlmaWNhdGVPZk9yaWdpbkFkZGl0aW9uYWxJbmZvO3hwAAAAAXBwcHBwcHQACjIwMDctMTItMDZ0AANVUFNwcHBwcHBwcHBwdAAAcQB+ABBzcgAcY29tLnVwcy5jb21tb24uY29yZS5TaGlwbWVudPHao0cAz4b9AgBBSgAOU2hpcG1lbnROdW1iZXJJAAxhY3R1YWxXZWlnaHRaABNhdmVyYWdlUGtnV2VpZ2h0SW5kWgAPYmlsbFRvRXhlbXB0aW9uSQAOYmlsbGFibGVXZWlnaHRJAA1iaWxsaW5nT3B0aW9uSQARZGltZW5zaW9uYWxXZWlnaHRaABxkb2N1bWVudHNPZk5vQ29tbWVyY2lhbFZhbHVlWgAMZXh0ZW5kZWRBcmVhWgASZnV0dXJlRGF0ZVNoaXBtZW50WgAZZ29vZHNOb3RJbkZyZWVDaXJjdWxhdGlvbloAC2lzVU9NTWV0cmljWgAUcmF0ZWRCeUh1bmRyZWRXZWlnaHRaABJyc0xhYmVsT3ZlcjEwMFBhY2tKAA5zZXF1ZW5jZU51bWJlcloAD3NoaXBtZW50UHJpY2luZ1oAEHNoaXBtZW50VXBsb2FkZWRaAAxzcGxpdER1dHlWYXRJAAt0b3RhbFdlaWdodFMABHpvbmVMABhRVk5CdW5kbGVDaGFyZ2VDb250YWluZXJ0ACVMY29tL3Vwcy9jb21tb24vY29yZS9DaGFyZ2VDb250YWluZXI7TAAcUVZOU2hpcEJ1bmRsZUNoYXJnZUNvbnRhaW5lcnEAfgASTAAGVU9NRGltcQB+AAJMAAlVT01XZWlnaHRxAH4AAkwAEWFiclJhdGVkSW5kaWNhdG9ydAAnTGNvbS91cHMvY29tbW9uL2NvcmUvQUJSUmF0ZWRJbmRpY2F0b3I7TAATYWNjQ2hhcmdlc0NvbnRhaW5lcnEAfgASTAAMYWNjQ29udGFpbmVydAAiTGNvbS91cHMvY29tbW9uL2NvcmUvQWNjQ29udGFpbmVyO0wAD2FsdERlbGl2ZXJ5VGltZXQAIExjb20vdXBzL2NvbW1vbi9jb3JlL0NvbW1pdFRpbWU7TAATYmlsbGluZ0N1cnJlbmN5Q29kZXEAfgACTAAKYnJsT3B0aW9uc3QAIExjb20vdXBzL2NvbW1vbi9jb3JlL0JSTE9wdGlvbnM7TAAKY2xpZW50SW5mb3QAIExjb20vdXBzL2NvbW1vbi9jb3JlL0NsaWVudEluZm87TAAKY29tbWl0VGltZXEAfgAVTAAOY29uc0xvY2F0aW9uSURxAH4AAkwADGN1cnJlbmN5Q29kZXEAfgACTAANZGF0ZVRpbWVTdGFtcHQAEExqYXZhL3V0aWwvRGF0ZTtMABJkZXNjcmlwdGlvbk9mR29vZHNxAH4AAkwACGRvY1N0YXRldAAjTGNvbS91cHMvY29tbW9uL2NvcmUvRG9jdW1lbnRTdGF0ZTtMABRmaXJzdFNlcnZpY2VTZWxlY3RlZHEAfgACTAAUZm9ybWF0dGVkT3V0cHV0U3BlY3N0ACpMY29tL3Vwcy9jb21tb24vY29yZS9Gb3JtYXR0ZWRPdXRwdXRTcGVjcztMABNncm91bmRUaW1laW5UcmFuc2l0dAAQTGphdmEvbGFuZy9Mb25nO0wAEWludEZvcm1EYXRhSG9sZGVydAAxTGNvbS91cHMvY29tbW9uL2NvcmUvSW50ZXJuYXRpb25hbEZvcm1EYXRhSG9sZGVyO0wAGWludGVybmF0aW9uYWxGb3Jtc0dyb3VwSURxAH4AAkwADWludkRhdGFIb2xkZXJ0ACdMY29tL3Vwcy9jb21tb24vY29yZS9JbnZvaWNlRGF0YUhvbGRlcjtMABFpbnZvaWNlTGluZVRvdGFsc3EAfgAbTAAJbWFpbGJveElkcQB+AAJMABhtYW5kYXRvcnlPUExEaW50ZXJuYWxLZXlxAH4AAkwAC3BhY2thZ2VMaXN0dAASTGphdmEvdXRpbC9WZWN0b3I7TAARcGF5ZXJPZkR1dHlBbmRUYXh0AB9MY29tL3Vwcy9jb21tb24vY29yZS9QYXllclR5cGU7TAAbcGF5ZXJPZlRyYW5zcG9ydGF0aW9uQ2hhcmdlcQB+AB9MABJwcm9tb3Rpb25Db250YWluZXJ0AChMY29tL3Vwcy9jb21tb24vY29yZS9Qcm9tb3Rpb25Db250YWluZXI7TAAOcmF0ZUNoYXJnZVR5cGV0ACBMY29tL3Vwcy9jb21tb24vY29yZS9DaGFyZ2VUeXBlO0wADXJhdGluZ09wdGlvbnN0ACNMY29tL3Vwcy9jb21tb24vY29yZS9SYXRpbmdPcHRpb25zO1sAB3JlZkxpc3R0ACBbTGNvbS91cHMvY29tbW9uL2NvcmUvUmVmZXJlbmNlO0wADnJlZ2lzdHJhdGlvbklkcQB+AAJMAAdzZWRJbmZvdAAdTGNvbS91cHMvY29tbW9uL2NvcmUvU0VESW5mbztMAAdzZXJ2aWNldAAdTGNvbS91cHMvY29tbW9uL2NvcmUvU2VydmljZTtMAApzaGlwVGlja2V0dAAkTGNvbS91cHMvY29tbW9uL2NvcmUvU2hpcHBpbmdUaWNrZXQ7TAAOc2hpcHBlckNvdW50cnlxAH4AAkwADXNoaXBwZXJOdW1iZXJxAH4AAkwAE3VwbG9hZERhdGVUaW1lU3RhbXBxAH4AGEwACHVzZXJEYXRhdAAmTGNvbS91cHMvY29tbW9uL2NvcmUvU2hpcG1lbnRVc2VyRGF0YTtMAAZ1c2VySURxAH4AAkwACHVzZXJuYW1lcQB+AAJMAAh2ZWNBZ2VudHEAfgAeTAANdm9pZEluZGljYXRvcnQAI0xjb20vdXBzL2NvbW1vbi9jb3JlL1ZvaWRJbmRpY2F0b3I7eHAAAAAAAAAAAAAAAlgAAAAAA4QAAAAKAAACWAAAAAAAAAAAAAAAC+zz1gAAAAAAAAAAynNyACNjb20udXBzLmNvbW1vbi5jb3JlLkNoYXJnZUNvbnRhaW5lciYzwMhRMUaMAgABTAAHY2hhcmdlc3QAD0xqYXZhL3V0aWwvTWFwO3hwc3IAEWphdmEudXRpbC5IYXNoTWFwBQfawcMWYNEDAAJGAApsb2FkRmFjdG9ySQAJdGhyZXNob2xkeHA/QAAAAAAAA3cIAAAABAAAAAFzcgAeY29tLnVwcy5jb21tb24uY29yZS5DaGFyZ2VUeXBlrR+VeQAj0rsCAAFJAAZtX3R5cGV4cAAAAG5zcgAaY29tLnVwcy5jb21tb24uY29yZS5DaGFyZ2UfQ7Bx7ElGngIAAkoABmFtb3VudEwACmNoYXJnZVR5cGVxAH4AIXhwAAAAAAAAAABxAH4AMHhzcQB+ACpzcQB+AC0/QAAAAAAAA3cIAAAABAAAAAFxAH4AMHNxAH4AMQAAAAAAAAAAcQB+ADB4dAACSU50AANMQlNzcgAlY29tLnVwcy5jb21tb24uY29yZS5BQlJSYXRlZEluZGljYXRvcheSnU0VawF5AgABSQALbV9pbmRpY2F0b3J4cAAAAAFzcQB+ACpzcQB+AC0/QAAAAAAAA3cIAAAABAAAAAFxAH4AMHNxAH4AMQAAAAAAAAAAcQB+ADB4c3IAIGNvbS51cHMuY29tbW9uLmNvcmUuQWNjQ29udGFpbmVycklff+hGsbUCAAFMAAZ2ZWNBY2NxAH4AHnhwc3IAEGphdmEudXRpbC5WZWN0b3LZl31bgDuvAQMAA0kAEWNhcGFjaXR5SW5jcmVtZW50SQAMZWxlbWVudENvdW50WwALZWxlbWVudERhdGF0ABNbTGphdmEvbGFuZy9PYmplY3Q7eHAAAAAAAAAAA3VyABNbTGphdmEubGFuZy5PYmplY3Q7kM5YnxBzKWwCAAB4cAAAAApzcgAyY29tLnVwcy5jb21tb24uY29yZS5TaGlwRW1haWxRdWFudHVtVmlld05vdGlmeVNoaXBdRnV48y5qDwIAAHhyACljb20udXBzLmNvbW1vbi5jb3JlLlNoaXBFbWFpbE5vdGlmaWNhdGlvbuHUupB3B1B+AgADTAAMZW1haWxBZGRyZXNzcQB+AAJMABNmYWlsdXJlRW1haWxBZGRyZXNzcQB+AAJMABNyZXBseVRvRW1haWxBZGRyZXNzcQB+AAJ4cgAkY29tLnVwcy5jb21tb24uY29yZS5TaGlwTm90aWZpY2F0aW9u5r6XfphJjUYCAAhMAAdkaWFsZWN0cQB+AAJMAAhsYW5ndWFnZXEAfgACTAAJbWVkaWFUeXBlcQB+AAJMAARtZW1vcQB+AAJMAAtyZXF1ZXN0VHlwZXEAfgACTAAMc2VudEZyb21OYW1lcQB+AAJMAAtzdWJqZWN0Q29kZXEAfgACTAALc3ViamVjdFRleHRxAH4AAnhyABtjb20udXBzLmNvbW1vbi5jb3JlLlNoaXBBY2MnN7nGY2TP0wIAAkwAD2NoYXJnZUNvbnRhaW5lcnEAfgASTAAWbWVyY2hhbmRpc2VEZXNjcmlwdGlvbnEAfgACeHIAHGNvbS51cHMuY29tbW9uLmNvcmUuU2hpcFByb3Bw3aE1Kl2BAQIAAHhyABxjb20udXBzLmNvbW1vbi5jb3JlLlByb3BlcnR5CXWtN8D8aDcCAAB4cHNxAH4AKnNxAH4ALT9AAAAAAAADdwgAAAAEAAAAAXEAfgAwc3EAfgAxAAAAAAAAAABxAH4AMHhwdAAAcQB+AE50AAIwM3QAE1FWTiBTaGlwIGVtYWlsIHRlc3R0AAMwMDZ0AA9UZXN0IFNtb2tlIFRlc3R0AAIwM3EAfgBOdAAPZHFkN21tbUB1cHMuY29tdAAPZHFkN21tbUB1cHMuY29tcQB+AE5zcgA3Y29tLnVwcy5jb21tb24uY29yZS5TaGlwRW1haWxRdWFudHVtVmlld05vdGlmeUV4Y2VwdGlvblnbQQuRrf6CAgAAeHEAfgBFc3EAfgAqc3EAfgAtP0AAAAAAAAN3CAAAAAQAAAABcQB+ADBzcQB+ADEAAAAAAAAAAHEAfgAweHBxAH4ATnEAfgBOcQB+AE9xAH4AUHQAAzAwN3EAfgBScQB+AFNxAH4ATnQAD2RxZDdtbW1AdXBzLmNvbXEAfgBVcQB+AE5zcgAhY29tLnVwcy5jb21tb24uY29yZS5TaGlwT0RTUGlja3Vw11GuqUBQPOUCAAlaABRoYXNCaWxsYWJsZU9EU1BpY2t1cEkAGXBpY2t1cFNjaGVkdWxpbmdJbmRpY2F0b3JMAAdvcmRlcklEcQB+AAJMAAtwaWNrdXBGbG9vcnEAfgACTAAOcGlja3VwTG9jYXRpb25xAH4AAkwAEHBpY2t1cFBvc3RhbENvZGVxAH4AAkwACnBpY2t1cFJvb21xAH4AAkwAE3BpY2t1cFdpbmRvd0VuZFRpbWV0AB1MamF2YS91dGlsL0dyZWdvcmlhbkNhbGVuZGFyO0wAFXBpY2t1cFdpbmRvd1N0YXJ0VGltZXEAfgBeeHEAfgBHc3EAfgAqc3EAfgAtP0AAAAAAAAN3CAAAAAQAAAABcQB+ADBzcQB+ADEAAAAAAAAAAHEAfgAweHAAAAAAAXEAfgBOdAABMnQABERlc2t0AAUzMjk2MHQAAzEwMHNyABtqYXZhLnV0aWwuR3JlZ29yaWFuQ2FsZW5kYXKPPdfW5bDQwQIAAUoAEGdyZWdvcmlhbkN1dG92ZXJ4cgASamF2YS51dGlsLkNhbGVuZGFy5upNHsjcW44DAAtaAAxhcmVGaWVsZHNTZXRJAA5maXJzdERheU9mV2Vla1oACWlzVGltZVNldFoAB2xlbmllbnRJABZtaW5pbWFsRGF5c0luRmlyc3RXZWVrSQAJbmV4dFN0YW1wSQAVc2VyaWFsVmVyc2lvbk9uU3RyZWFtSgAEdGltZVsABmZpZWxkc3QAAltJWwAFaXNTZXR0AAJbWkwABHpvbmV0ABRMamF2YS91dGlsL1RpbWVab25lO3hwAQAAAAEBAQAAAAEAAAAHAAAAAQAAARbGSZjLdXIAAltJTbpgJnbqsqUCAAB4cAAAABEAAAABAAAH1wAAAAsAAAAyAAAAAwAAAAoAAAFYAAAAAgAAAAIAAAABAAAABgAAABIAAAAAAAAAFAAAASv+7VeAAAAAAHVyAAJbWlePIDkUuF3iAgAAeHAAAAARAQEBAQEBAQEBAQEBAQEBAQFzcgAYamF2YS51dGlsLlNpbXBsZVRpbWVab25l+mddYNFe9aYDABJJAApkc3RTYXZpbmdzSQAGZW5kRGF5SQAMZW5kRGF5T2ZXZWVrSQAHZW5kTW9kZUkACGVuZE1vbnRoSQAHZW5kVGltZUkAC2VuZFRpbWVNb2RlSQAJcmF3T2Zmc2V0SQAVc2VyaWFsVmVyc2lvbk9uU3RyZWFtSQAIc3RhcnREYXlJAA5zdGFydERheU9mV2Vla0kACXN0YXJ0TW9kZUkACnN0YXJ0TW9udGhJAAlzdGFydFRpbWVJAA1zdGFydFRpbWVNb2RlSQAJc3RhcnRZZWFyWgALdXNlRGF5bGlnaHRbAAttb250aExlbmd0aHQAAltCeHIAEmphdmEudXRpbC5UaW1lWm9uZTGz6fV3RKyhAgABTAACSURxAH4AAnhwdAAQQW1lcmljYS9OZXdfWW9yawA27oAAAAABAAAAAQAAAAMAAAAKAG3dAAAAAAD+7VeAAAAAAgAAAAIAAAABAAAAAwAAAAIAbd0AAAAAAAAAAAABdXIAAltCrPMX+AYIVOACAAB4cAAAAAwfHB8eHx4fHx4fHh93CgAAAAYIAQEBAAB1cQB+AG0AAAACAG3dAABt3QB4c3IAGnN1bi51dGlsLmNhbGVuZGFyLlpvbmVJbmZvJNHTzgAdcZsCAAhJAAhjaGVja3N1bUkACmRzdFNhdmluZ3NJAAlyYXdPZmZzZXRJAA1yYXdPZmZzZXREaWZm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aoEBS1l0GQMAAHhwdwgAAAEWsOeoqXhxAH4ATnNyACFjb20udXBzLmNvbW1vbi5jb3JlLkRvY3VtZW50U3RhdGUm3FaM/tZGJwIAAUkAD2N1cnJlbnREb2NTdGF0ZXhwAAAAA3EAfgBOc3IAKGNvbS51cHMuY29tbW9uLmNvcmUuRm9ybWF0dGVkT3V0cHV0U3BlY3OavO3S8eSk6gIAF1oACGN1dExhYmVsWgANZ2VuZXJhdGVMYWJlbFoAD2dlbmVyYXRlUmVjZWlwdFoAF2dlbmVyYXRlVXBzY29weUh0bWxUZXh0WgAMaGlkZUFCUlJhdGVzWgAOaGlkZUFjY3ROdW1JbmRaAAxoaWRlUmF0ZXNJbmRJAAtsYWJlbEhlaWdodEkACmxhYmVsV2lkdGhaAA1ub25JbnRlcmxhY2VkWgAOcHJpbnRMYWJlbEljb25aAAZzYW1wbGVMAAdjaGFyU2V0dAArTGNvbS91cHMvY29tbW9uL2NvcmUvQ2hhcmFjdGVyU2V0SW5kaWNhdG9yO0wAEWNvbnRyb2xMb2dCYXNlVVJMcQB+AAJMAA1odHRwVXNlckFnZW50dAAfTGNvbS91cHMvY29tbW9uL2NvcmUvVXNlckFnZW50O0wADGxhYmVsQmFzZVVSTHEAfgACTAAObGFiZWxJbWFnZVR5cGVxAH4AAkwAF3BhY2thZ2VOdW1Ub0dlbkxhYmVsRm9ycQB+AB5MABByZWNlaXB0SW1hZ2VUeXBlcQB+AAJMABl0cmFuc2xhdGVkRG9jdW1lbnRDb250ZW50dAAvTGNvbS91cHMvY29tbW9uL2NvcmUvVHJhbnNsYXRlZERvY3VtZW50Q29udGVudDtMABh1cHNDb3B5VHVybmluQ29weUJhc2VVUkxxAH4AAkwAGnVwc0NvcHlUdXJuaW5Db3B5SW1hZ2VUeXBlcQB+AAJMAA53YXJzYXdUZXh0TGFuZ3QAKExjb20vdXBzL2NvbW1vbi9jb3JlL1dhcnNhd1RleHRMYW5ndWFnZTt4cAABAAABAAAAAAAGAAAABAABAHNyACljb20udXBzLmNvbW1vbi5jb3JlLkNoYXJhY3RlclNldEluZGljYXRvciQBq91ppCJWAgAAeHIAK2NvbS51cHMuY29tbW9uLmNvcmUudXRpbC5BYnN0cmFjdFN0cmluZ0VudW3h0XiTT8IesgIAAkwACWNsYXNzTmFtZXEAfgACTAALc3RyaW5nVmFsdWVxAH4AAnhwdAApY29tLnVwcy5jb21tb24uY29yZS5DaGFyYWN0ZXJTZXRJbmRpY2F0b3J0AAZMYXRpbjF0AAEvc3IAHWNvbS51cHMuY29tbW9uLmNvcmUuVXNlckFnZW50UAgAb1NgIT8CABpJAAZjdXJQb3NMAAhidWlsZE51bXEAfgACTAAJYnVpbGROdW0ycQB+AAJMAAZsQWdlbnRxAH4AAkwACW1hak1velZlcnEAfgACTAAIbWFqT1NWZXJxAH4AAkwACm1halBsYXRWZXJxAH4AAkwACG1ham9yVmVycQB+AAJMAAltYWpvclZlcjJxAH4AAkwACW1pbk1velZlcnEAfgACTAAIbWluT1NWZXJxAH4AAkwACm1pblBsYXRWZXJxAH4AAkwACG1pbm9yVmVycQB+AAJMAAltaW5vclZlcjJxAH4AAkwACm1velZlcnNpb250ABJMamF2YS9sYW5nL0RvdWJsZTtMAARuYW1lcQB+AAJMAAJvc3EAfgACTAAJb3NWZXJzaW9ucQB+AKNMAAZvc3R5cGVxAH4AAkwAC3BsYXRWZXJzaW9ucQB+AKNMAAhwbGF0Zm9ybXEAfgACTAAJdXNlckFnZW50cQB+AAJMAAd2YXJpYW50cQB+AAJMAAZ2ZW5kb3JxAH4AAkwAB3ZlcnNpb25xAH4Ao0wACHZlcnNpb24ycQB+AKN4cP////9xAH4ATnEAfgBOdAALbW96aWxsYS80LjV0AAE0cQB+AE5xAH4ATnEAfgCmcQB+AE50AAE1cQB+AE5xAH4ATnEAfgCncQB+AE5zcgAQamF2YS5sYW5nLkRvdWJsZYCzwkopa/sEAgABRAAFdmFsdWV4cgAQamF2YS5sYW5nLk51bWJlcoaslR0LlOCLAgAAeHBAEgAAAAAAAHQAAk5WcQB+AE5zcQB+AKi/8AAAAAAAAHEAfgBOcQB+AKxxAH4ATnQAC01vemlsbGEvNC41cQB+AE50AAhOZXRzY2FwZXEAfgCqcQB+AKx0ABouL2xhYmVsLS1CVFJBQy0tLi0tQklNVFktLXQAA2dpZnNxAH4APwAAAAoAAAAAdXEAfgBCAAAACnBwcHBwcHBwcHB4dAAEZXBsMnNyAC1jb20udXBzLmNvbW1vbi5jb3JlLlRyYW5zbGF0ZWREb2N1bWVudENvbnRlbnSKVkulwSPylQIAAloABnNhbXBsZUwADGNvbnRlbnRUYWJsZXEAfgAreHAAc3EAfgAtP0AAAAAAABh3CAAAACAAAAASdAAcU3RhdGljQ29udGVudF9EYXRlT2ZTaGlwbWVudHQAEERhdGUgb2YgU2hpcG1lbnR0ACNTdGF0aWNDb250ZW50X0RhaWx5UGlja3VwQ3VzdG9tZXJzMnQAMllvdXIgZHJpdmVyIHdpbGwgcGlja3VwIHlvdXIgc2hpcG1lbnQocykgYXMgdXN1YWwudAAXU3RhdGljQ29udGVudF9TaWduYXR1cmV0ABNTaGlwcGVyJ3MgU2lnbmF0dXJldAAaU3RhdGljQ29udGVudF9JbnZvaWNlX1RleHR0AQktIDMgY29waWVzIG9mIGEgY29tcGxldGVkIGN1c3RvbXMgaW52b2ljZSBhcmUgcmVxdWlyZWQgZm9yIHNoaXBtZW50cyB3aXRoIGEgY29tbWVyY2lhbCB2YWx1ZSBiZWluZyBzaGlwcGVkIHRvL2Zyb20gbm9uLUVVIGNvdW50cmllcy4gIFBsZWFzZSBpbnN1cmUgdGhlIGN1c3RvbXMgaW52b2ljZSBjb250YWlucyBhZGRyZXNzIGluZm9ybWF0aW9uLCBwcm9kdWN0IGRldGFpbCAtIGluY2x1ZGluZyB2YWx1ZSwgc2hpcG1lbnQgZGF0ZSBhbmQgeW91ciBzaWduYXR1cmUudAATU3RhdGljQ29udGVudF9QcmludHQAEFByaW50IHRoZSBsYWJlbDp0ACZTdGF0aWNDb250ZW50X0N1c3RvbWVyc1dpdGhEYWlseVBpY2t1cHQAHUN1c3RvbWVycyB3aXRoIGEgRGFpbHkgUGlja3VwdAAVTGFiZWxfU0hJUFBFUl9SRUxFQVNFdAAlQXR0ZW50aW9uIFVQUyBEcml2ZXI6IFNISVBQRVIgUkVMRUFTRXQAHlN0YXRpY0NvbnRlbnRfR3JvdW5kM0RheVNlbGVjdHQBNFRha2UgdGhpcyBwYWNrYWdlIHRvIGFueSBsb2NhdGlvbiBvZiBUaGUgVVBTIFN0b3Jlwq4sIFVQUyBEcm9wIEJveCwgVVBTIEN1c3RvbWVyIENlbnRlciwgVVBTIEFsbGlhbmNlcyAoT2ZmaWNlIERlcG90wq4gb3IgU3RhcGxlc8KuKSBvciBBdXRob3JpemVkIFNoaXBwaW5nIE91dGxldCBuZWFyIHlvdSBvciB2aXNpdCA8YSBocmVmPSJodHRwOi8vaHR0cDovL3d3dy51cHMuY29tL2NvbnRlbnQvdXMvZW4vaW5kZXguanN4Ij5odHRwOi8vd3d3LnVwcy5jb20vY29udGVudC91cy9lbi9pbmRleC5qc3g8L2E+IGFuZCBzZWxlY3QgRHJvcCBPZmYudAAYU3RhdGljQ29udGVudF9BY2NlcHRhbmNldADrVG8gYWNrbm93bGVkZ2UgeW91ciBhY2NlcHRhbmNlIG9mIHRoZSBvcmlnaW5hbCBsYW5ndWFnZSBvZiB0aGUgYWdyZWVtZW50IHdpdGggVVBTIGFzIHN0YXRlZCBvbiB0aGUgY29uZmlybSBwYXltZW50IHBhZ2UsIGFuZCB0byBhdXRob3JpemUgVVBTIHRvIGFjdCBhcyBmb3J3YXJkaW5nIGFnZW50IGZvciBleHBvcnQgY29udHJvbCBhbmQgY3VzdG9tIHB1cnBvc2VzLCA8Yj5zaWduIGFuZCBkYXRlIGhlcmU6PC9iPnQAG1N0YXRpY0NvbnRlbnRfSW52b2ljZV9UaXRsZXQAEEN1c3RvbXMgSW52b2ljZSB0ABlQYWdlVGl0bGVzX0xhYmVsUGFnZVRpdGxldAAQVmlldy9QcmludCBMYWJlbHQAGlN0YXRpY0NvbnRlbnRfQWlyU2hpcG1lbnRzdADLQWlyIHNoaXBtZW50cyAoaW5jbHVkaW5nIFdvcmxkd2lkZSBFeHByZXNzIGFuZCBFeHBlZGl0ZWQpIGNhbiBiZSBwaWNrZWQgdXAgb3IgZHJvcHBlZCBvZmYuIFRvIHNjaGVkdWxlIGEgcGlja3VwLCBvciB0byBmaW5kIGEgZHJvcC1vZmYgbG9jYXRpb24sIHNlbGVjdCB0aGUgUGlja3VwIG9yIERyb3Atb2ZmIGljb24gZnJvbSB0aGUgVVBTIHRvb2wgYmFyLiB0ABtTdGF0aWNDb250ZW50X1ByaW50U2VudGVuY2V0AFBTZWxlY3QgUHJpbnQgZnJvbSB0aGUgRmlsZSBtZW51IGluIHRoaXMgYnJvd3NlciB3aW5kb3cgdG8gcHJpbnQgdGhlIGxhYmVsIGJlbG93LnQAGlN0YXRpY0NvbnRlbnRfRm9sZFNlbnRlbmNldACUUGxhY2UgdGhlIGxhYmVsIGluIGEgVVBTIFNoaXBwaW5nIFBvdWNoLiBJZiB5b3UgZG8gbm90IGhhdmUgYSBwb3VjaCwgYWZmaXggdGhlIGZvbGRlZCBsYWJlbCB1c2luZyBjbGVhciBwbGFzdGljIHNoaXBwaW5nIHRhcGUgb3ZlciB0aGUgZW50aXJlIGxhYmVsLnQAJlN0YXRpY0NvbnRlbnRfR2V0dGluZ1lvdXJTaGlwbWVudFRvVVBTdAAcR0VUVElORyBZT1VSIFNISVBNRU5UIFRPIFVQU3QAElN0YXRpY0NvbnRlbnRfRm9sZHQAKkZvbGQgdGhlIHByaW50ZWQgbGFiZWwgYXQgdGhlIGRvdHRlZCBsaW5lLnQAFlN0YXRpY0NvbnRlbnRfRm9sZEhlcmV0AAlGT0xEIEhFUkV0AChTdGF0aWNDb250ZW50X0N1c3RvbWVyc1dpdGhOb0RhaWx5UGlja3VwdAAgQ3VzdG9tZXJzIHdpdGhvdXQgYSBEYWlseSBQaWNrdXB4cQB+AKF0AARodG1sc3IAJmNvbS51cHMuY29tbW9uLmNvcmUuV2Fyc2F3VGV4dExhbmd1YWdlPbpgl71I/osCAAB4cQB+AJ10ACZjb20udXBzLmNvbW1vbi5jb3JlLldhcnNhd1RleHRMYW5ndWFnZXQAAjEwcHBxAH4ATnBwdAAISU5URVJORVRxAH4ATnNxAH4APwAAAAAAAAABdXEAfgBCAAAABXNyAB9jb20udXBzLmNvbW1vbi5jb3JlLlNoaXBQYWNrYWdljcNsyBVp7a0CABxJAA5iaWxsYWJsZVdlaWdodEkACWRpbVdlaWdodFoAFWxhcmdlUGFja2FnZVN1cmNoYXJnZVMACG92ZXJzaXplSgAJcGFja2FnZUlkWgAYcGtnRGltU3VyY2hhcmdlSW5kaWNhdG9ySQARcGtnU2VxdWVuY2VOdW1iZXJJAAZ3ZWlnaHRMABhRVk5CdW5kbGVDaGFyZ2VDb250YWluZXJxAH4AEkwAHFFWTlNoaXBCdW5kbGVDaGFyZ2VDb250YWluZXJxAH4AEkwAFlJTTXVsdGlQaWVjZVNoaXBtZW50SWRxAH4AAkwAE2FjY0NoYXJnZXNDb250YWluZXJxAH4AEkwADGFjY0NvbnRhaW5lcnEAfgAUTAAKYWN0aXZpdGllc3EAfgAeTAANZGF0ZVRpbWVTdGFtcHEAfgAYTAAbaGF6TWF0QnVuZGxlQ2hhcmdlQ29udGFpbmVycQB+ABJMABZtZXJjaGFuZGlzZURlc2NyaXB0aW9ucQB+AAJMAA5wYWNrYWdlT1BMREtleXEAfgACTAALcGFja2FnZVR5cGVxAH4AAkwAD3BhY2thZ2VUeXBlU2l6ZXQAJUxjb20vdXBzL2NvbW1vbi9jb3JlL1BhY2thZ2VUeXBlU2l6ZTtMAAZwYXJlbnRxAH4AA0wAB3BrZ0RpbXN0ACNMY29tL3Vwcy9jb21tb24vY29yZS9Qa2dEaW1lbnNpb25zO1sAB3JlZkxpc3RxAH4AI0wAFnNlcnZpY2VDaGFyZ2VDb250YWluZXJxAH4AEkwADHNpaUluZGljYXRvcnEAfgACTAAUdG90YWxDaGFyZ2VDb250YWluZXJxAH4AEkwADnRyYWNraW5nTnVtYmVycQB+AAJMAA12b2lkSW5kaWNhdG9ycQB+ACh4cAAAA4QAAAJYAQAEAAAAAAAAAAAA/////wAAAlhzcQB+ACpzcQB+AC0/QAAAAAAAA3cIAAAABAAAAAFxAH4AMHNxAH4AMQAAAAAAAAAAcQB+ADB4c3EAfgAqc3EAfgAtP0AAAAAAAAN3CAAAAAQAAAABcQB+ADBzcQB+ADEAAAAAAAAAAHEAfgAweHBzcQB+ACpzcQB+AC0/QAAAAAAAA3cIAAAABAAAAAFxAH4AMHNxAH4AMQAAAAAAAAAAcQB+ADB4c3EAfgA9c3EAfgA/AAAAAAAAAAN1cQB+AEIAAAAKc3IAIGNvbS51cHMuY29tbW9uLmNvcmUuTGFyZ2VQYWNrYWdlOqRYaWr3m4UCAAB4cgAjY29tLnVwcy5jb21tb24uY29yZS5BY2Nlc3NvcmlhbEltcGziwrg3sxU8uQIAAkkAB2FjY1R5cGVMAA9jaGFyZ2VDb250YWluZXJxAH4AEnhwAAABeXNxAH4AKnNxAH4ALT9AAAAAAAADdwgAAAAEAAAAAXEAfgAwc3EAfgAxAAAAAAAAD6BxAH4AMHhzcgAgY29tLnVwcy5jb21tb24uY29yZS5EZWxpdmVyeUFyZWGZERLSvWYiCgIAAUwAGWRlbGl2ZXJ5QXJlYVN1cmNoYXJnZVR5cGV0AC9MY29tL3Vwcy9jb21tb24vY29yZS9EZWxpdmVyeUFyZWFTdXJjaGFyZ2VUeXBlO3hxAH4A9AAAAXhzcQB+ACpzcQB+AC0/QAAAAAAAA3cIAAAABAAAAAFxAH4AMHNxAH4AMQAAAAAAAAAAcQB+ADB4c3IALWNvbS51cHMuY29tbW9uLmNvcmUuRGVsaXZlcnlBcmVhU3VyY2hhcmdlVHlwZd57tY5gmVCWAgAAeHEAfgCddAAtY29tLnVwcy5jb21tb24uY29yZS5EZWxpdmVyeUFyZWFTdXJjaGFyZ2VUeXBldAAIU1VCVVJCQU5zcgAhY29tLnVwcy5jb21tb24uY29yZS5GdWVsU3VyY2hhcmdl/cTgpvhK/SwCAAB4cQB+APQAAAF3c3EAfgAqc3EAfgAtP0AAAAAAAAN3CAAAAAQAAAABcQB+ADBzcQB+ADEAAAAAAAAGjnEAfgAweHBwcHBwcHB4c3EAfgA/AAAAAAAAAAB1cQB+AEIAAAAKcHBwcHBwcHBwcHhwcHQAE0ludGVybmF0aW9uYWwgR29vZHNxAH4ATnQAAjAycHEAfgApc3IAIWNvbS51cHMuY29tbW9uLmNvcmUuUGtnRGltZW5zaW9uc75ta3hVvFQ6AgADUwAGaGVpZ2h0UwAGbGVuZ3RoUwAFd2lkdGh4cgAbY29tLnVwcy5jb21tb24uY29yZS5Qa2dQcm9wv7Az2mcXCZQCAAB4cQB+AEkAAAAAAAB1cgAgW0xjb20udXBzLmNvbW1vbi5jb3JlLlJlZmVyZW5jZTtbsG+cfhdIXQIAAHhwAAAABnNyAB1jb20udXBzLmNvbW1vbi5jb3JlLlJlZmVyZW5jZdzYzgdAdS3nAgAGSQACSURaAA1sYWJlbEJhckNvZGVkTAANYmFyY29kZU1ldGhvZHEAfgACTAAHcmVmQ29kZXEAfgACTAAHcmVmTmFtZXEAfgACTAAEdGV4dHEAfgACeHAAAAABAHQAAzAwOHQAAjAwdAAOUmVmZXJlbmNlIE5vLjF0AAYwMXBwbXNzcQB+AREAAAACAHEAfgETdAACMDB0AA1SZWZlcmVuY2UgIyAycHBwcHBzcQB+ACpzcQB+AC0/QAAAAAAAA3cIAAAABAAAAAFxAH4AMHNxAH4AMQAAAAAAAC5IcQB+ADB4cQB+AE5zcQB+ACpzcQB+AC0/QAAAAAAAA3cIAAAABAAAAAFxAH4AMHNxAH4AMQAAAAAAAC5IcQB+ADB4dAASMVoyMjIwMDYwMjkyMzUzODI5cHBwcHB4cHNyAB1jb20udXBzLmNvbW1vbi5jb3JlLlBheWVyVHlwZdpgzJsj4RiyAgAAeHIAKGNvbS51cHMuY29tbW9uLmNvcmUudXRpbC5BYnN0cmFjdEludEVudW2pHR4EvN667QIAAkkACGludFZhbHVlTAAJY2xhc3NOYW1lcQB+AAJ4cAAAAAp0AB1jb20udXBzLmNvbW1vbi5jb3JlLlBheWVyVHlwZXBzcQB+AC8AAAB4c3IAIWNvbS51cHMuY29tbW9uLmNvcmUuUmF0aW5nT3B0aW9ucxlyIuaXmnCuAgAIWgASYWNjb3VudEJhc2VkUmF0aW5nWgAOY2FsY3VsYXRlVGF4ZXNaAAhpc1Vwc2VsbEkACXJhdGVDaGFydEwADGFjY2Vzc01ldGhvZHEAfgACTAALYmlsbGluZ1RpZXJxAH4AAkwADWh1bmRyZWRXZWlnaHR0ABNMamF2YS9sYW5nL0ludGVnZXI7TAAKcGlja3VwRGF0ZXEAfgAYeHABAAAAAAABdAADUkVHcHNyABFqYXZhLmxhbmcuSW50ZWdlchLioKT3gYc4AgABSQAFdmFsdWV4cQB+AKkAAAABc3EAfgCSdwgAAAEWxCRHynh1cQB+AQ8AAAACc3EAfgERAAAAAQBxAH4BE3EAfgEYdAANUmVmZXJlbmNlICMgMXBzcQB+AREAAAACAHEAfgETcQB+ARh0AA1SZWZlcmVuY2UgIyAycHQACklTMDAzNDI5MDVwc3IAG2NvbS51cHMuY29tbW9uLmNvcmUuU2VydmljZZGF5NRJIjDyAgADTAAPY2hhcmdlQ29udGFpbmVycQB+ABJMAAZyYXZlSWRxAH4AAkwABHR5cGVxAH4AAnhwc3EAfgAqc3EAfgAtP0AAAAAAAAN3CAAAAAQAAAABcQB+ADBzcQB+ADEAAAAAAAAuSHEAfgAweHQAAzJEQXQAAzAwMnBxAH4ATnQABjIyMjAwNnBwdAAGVVNBQlIxcHNxAH4APwAAAAAAAAADdXEAfgBCAAAACnNyABljb20udXBzLmNvbW1vbi5jb3JlLkFnZW50wS/XxZ7wK0UCABNaAA9iaWxsVG9FeGVtcHRpb25JAARyb2xlTAACSURxAH4AAkwAClBDUGhvbmVOdW1xAH4AAkwAB2FkZHJlc3N0AB1MY29tL3Vwcy9jb21tb24vY29yZS9BZGRyZXNzO0wAEGJhbGFuY2VDb250YWluZXJxAH4AEkwABWVtYWlscQB+AAJMAAlleHRlbnNpb25xAH4AAkwAA2ZheHQAH0xjb20vdXBzL2NvbW1vbi9jb3JlL0ZheE51bWJlcjtMABlncmFuZFRvdGFsQ2hhcmdlQ29udGFpbmVycQB+ABJMAARuYW1lcQB+AAJMAAhuaWNrTmFtZXEAfgACTAAOcGF5bWVudEFjY291bnR0AB1MY29tL3Vwcy9jb21tb24vY29yZS9BY2NvdW50O0wABXBob25lcQB+AAJMAA9zaGlwcGluZ0FjY291bnRxAH4BQUwACXRheERldGFpbHQAIkxjb20vdXBzL2NvbW1vbi9jb3JlL1RheENvbnRhaW5lcjtMAAV0YXhJZHEAfgACTAAJdGF4SWRUeXBlcQB+AAJMABd0YXhUb3RhbENoYXJnZUNvbnRhaW5lcnEAfgASeHAAAAAACnEAfgBOcQB+AE5zcgAbY29tLnVwcy5jb21tb24uY29yZS5BZGRyZXNz6eSttx6T5gACABNEAAlBVlF1YWxpdHlaABdjb25zaWduZWVCaWxsaW5nRW5hYmxlZFoAC3Jlc2lkZW50aWFsTAAFYWRkcjJxAH4AAkwABWFkZHIzcQB+AAJMAAlhZGRyU2F2ZWRxAH4AAkwAF2FkZHJTdGFuZGFyZGl6YXRpb25UeXBlcQB+AAJMABVhZGRyVmFsaWRhdGlvblJlc3VsdHNxAH4AAkwABGNpdHlxAH4AAkwAC2NvbnRhY3ROYW1lcQB+AAJMAAdjb3VudHJ5cQB+AAJMABFkYXRhQ2FwdHVyZU1ldGhvZHEAfgACTAAKbG9jYXRpb25JRHEAfgACTAAKcG9zdGFsQ29kZXEAfgACTAAMcG9zdGFsQ29kZUhpcQB+AAJMAAxwb3N0YWxDb2RlTG9xAH4AAkwABXN0YXRlcQB+AAJMAAZzdHJlZXRxAH4AAkwADHVyYmFuaXphdGlvbnEAfgACeHAAAAAAAAAAAAAAcQB+AE5xAH4ATnEAfgBOcQB+AE5xAH4ATnQACFRpbW9uaXVtcQB+AE50AAJVU3EAfgBOcQB+AE50AAUyMTA5M3EAfgBOcQB+AE50AAJNRHQAEzIgU291dGggTWFpbiBTdHJlZXRxAH4ATnNxAH4AKnNxAH4ALT9AAAAAAAADdwgAAAAEAAAAAnEAfgElc3EAfgAxAAAAAAAALPpxAH4BJXEAfgAwc3EAfgAxAAAAAAAALkhxAH4AMHhxAH4ATnEAfgBOc3IAHWNvbS51cHMuY29tbW9uLmNvcmUuRmF4TnVtYmVy1csn6BpfcEwCAAJaAA9pc0ludGVybmF0aW9uYWxMAAZudW1iZXJxAH4AAnhwAHEAfgBOc3EAfgAqc3EAfgAtP0AAAAAAAAN3CAAAAAQAAAABcQB+ASVzcQB+ADEAAAAAAAAs+nEAfgEleHQAElJvY2tldCBKLiBTcXVpcnJlbHEAfgBOc3IAHmNvbS51cHMuY29tbW9uLmNvcmUuVVBTQWNjb3VudLO56+H5AAuMAgADTAAHY291bnRyeXEAfgACTAALZGVzY3JpcHRpb25xAH4AAkwACnBvc3RhbENvZGVxAH4AAnhyABtjb20udXBzLmNvbW1vbi5jb3JlLkFjY291bnQql4BlwlQtuQIABEwAB0NQU1R5cGVxAH4AAkwADWFjY291bnROdW1iZXJxAH4AAkwAGmN1c3RvbWVyQ2xhc3NpZmljYXRpb25Db2RlcQB+AAJMAAZzdGF0dXNxAH4AAnhwcQB+AE50AAYyMjIwMDZxAH4ATnEAfgBOcQB+AE5xAH4ATnEAfgBOdAAKMTIzNDU2Nzg5MHNxAH4BVXQAAjAxcQB+ATp0AAIwMXQAAjAxdAACVVNxAH4ATnEAfgBOcHQACjEyMzQ1Njc4NzdxAH4ATnNxAH4AKnNxAH4ALT9AAAAAAAADdwgAAAAEAAAAAXEAfgElc3EAfgAxAAAAAAAAAABxAH4BJXhzcQB+AT4AAAAAHnEAfgBOcQB+AE5zcQB+AUQAAAAAAAAAAAAAdAAGaGVoZWhldAAUVGhyZWUgbGluZXMgaGVyZSB0b29xAH4ATnEAfgBOcQB+AE50AApWZXJvIEJlYWNodAAOTWFybGV5IEJyaW5zb250AAJVU3EAfgBOcQB+AE50AAUzMjk2MHEAfgBOcQB+AE50AAJGTHQAEzIwMDQgR3JlZW5zcHJpbmcgRHJxAH4ATnNxAH4AKnNxAH4ALT9AAAAAAAADdwgAAAAEAAAAAXEAfgAwc3EAfgAxAAAAAAAAAABxAH4AMHhxAH4ATnEAfgBOc3EAfgFPAHEAfgBOcHQAFEhhcHB5IERvZyBQZXQgU3VwcGx5cQB+AE5wdAALOTcyMjUzNzcxNzFwcHEAfgBOcQB+AE5wc3EAfgE+AAAAABRxAH4ATnEAfgBOc3EAfgFEAAAAAAAAAAAAAHQABmhlaGVoZXQAFFRocmVlIGxpbmVzIGhlcmUgdG9vcQB+AE5xAH4ATnEAfgBOdAAKVmVybyBCZWFjaHQAFFN1Z2FyIEJvb2dlciBUZXRyaWNrdAACVVNxAH4ATnEAfgBOcQB+AGVxAH4ATnEAfgBOdAACRkx0ABMyMDA0IEdyZWVuc3ByaW5nIERycQB+AE5zcQB+ACpzcQB+AC0/QAAAAAAAA3cIAAAABAAAAAFxAH4AMHNxAH4AMQAAAAAAAAAAcQB+ADB4cQB+AE5xAH4ATnNxAH4BTwBxAH4ATnB0ABNCdWxsd2lua2xlIEouIE1vb3NlcQB+AE5wdAAKMTIzNDU2Nzg5MHBwdAAKMTIzNDU2Nzg3N3EAfgBOcHBwcHBwcHB4cA==</ShipmentDigest>");
            sb.Append("</ShipmentAcceptRequest>");
            return sb.ToString();
        }

        #endregion

        #region Address Validation
        public string AddressValidation(Address objAddress)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version='1.0'?>");
            sb.Append("	<AccessRequest xml:lang='en-US'>");
            sb.Append("		<AccessLicenseNumber>").Append(AccessNumber).Append("</AccessLicenseNumber>");
            sb.Append("		<UserId>").Append(UserName).Append("</UserId>");
            sb.Append("		<Password>").Append(Password).Append("</Password>");
            sb.Append("	</AccessRequest>");
            sb.Append("<?xml version=\"1.0\" ?>");
            sb.Append("<AddressValidationRequest xml:lang=\"en-US\">");
            sb.Append("<Request><TransactionReference>");
            sb.Append("<XpciVersion>1.0001</XpciVersion>");
            sb.Append("</TransactionReference>");
            sb.Append("<RequestAction>AV</RequestAction>");
            sb.Append("</Request>");
            sb.Append("<Address>");
            sb.Append("<City>" + GetMaxAllowedString(objAddress.City, 30) + "</City>");
            sb.Append("<StateProvinceCode>" + GetMaxAllowedString(objAddress.State.StateCode, 5) + "</StateProvinceCode>");
            sb.Append("<CountryCode>" + GetMaxAllowedString(objAddress.Country.CountryCode, 2) + "</CountryCode>");
            sb.Append("<PostalCode>" + GetMaxAllowedString(objAddress.ZipCode, 10) + "</PostalCode>");
            sb.Append("</Address>");
            sb.Append("</AddressValidationRequest>");
            return sb.ToString();
        }

        public static IEnumerable<AddressValidationResponse> GetAddressValidationResponses(string response, ref string error)
        {
            var addressList = new List<AddressValidationResponse>();

            var addressValidationObj = new AddressValidationResponse();
            Error errorObj = new Error();

            using (var sr = new StringReader(response))
            using (var tr = new XmlTextReader(sr))
                while (tr.Read())
                {
                    if ((tr.Name == "AddressValidationResponse") && (tr.NodeType == XmlNodeType.Element))
                    {
                        while (tr.Read())
                        {
                            if ((tr.Name == "Response") && (tr.NodeType == XmlNodeType.Element))
                            {
                                while (tr.Read())
                                {


                                    if ((tr.Name == "ResponseStatusCode") && (tr.NodeType == XmlNodeType.Element))
                                    {
                                        addressValidationObj.StatusCode = tr.ReadString();
                                        tr.ReadEndElement();
                                    }
                                    if ((tr.Name == "ResponseStatusDescription") && (tr.NodeType == XmlNodeType.Element))
                                    {
                                        addressValidationObj.StatusDescription = tr.ReadString();
                                        tr.ReadEndElement();
                                    }
                                    if ((tr.Name == "ResponseStatusCode") && (tr.NodeType == XmlNodeType.EndElement))
                                    {
                                        break;
                                    }
                                    if ((tr.Name == "ResponseStatusDescription") && (tr.NodeType == XmlNodeType.EndElement))
                                    {
                                        break;
                                    }

                                    if ((tr.Name == "Error") && (tr.NodeType == XmlNodeType.Element))
                                    {
                                        while (tr.Read())
                                        {
                                            if ((tr.Name == "ErrorCode") && (tr.NodeType == XmlNodeType.Element))
                                            {
                                                errorObj.Code = tr.ReadString();
                                            }
                                            if ((tr.Name == "ErrorDescription") && (tr.NodeType == XmlNodeType.Element))
                                            {
                                                errorObj.Description = tr.ReadString();
                                            }

                                        }
                                    }

                                }
                            }
                        }
                        addressValidationObj.Error = errorObj;
                        addressList.Add(addressValidationObj);
                    }
                }

            return addressList;
        }

        #endregion

        #region Private Method
        public static XmlDocument LoadXML(string xmlString)
        {
            XmlDocument xml = new XmlDocument();

            if (xmlString != null)
                xml.LoadXml(xmlString);

            return xml;
        }

        public static string GetMaxAllowedString(string str, int maxLength)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(str))
                return str;
            else
            {
                if (str.Length > maxLength)
                    result = str.Substring(0, maxLength);
                else
                    result = str;

                return result;
            }
        }

        public static string GetServiceName(string serviceId)
        {
            switch (serviceId)
            {
                case "01":
                    return "UPS Next Day Air";
                case "02":
                    return "UPS 2nd Day Air";
                case "03":
                    return "UPS Ground";
                case "07":
                    return "UPS Worldwide Express";
                case "08":
                    return "UPS Worldwide Expedited";
                case "11":
                    return "UPS Standard";
                case "12":
                    return "UPS 3 Day Select";
                case "13":
                    return "UPS Next Day Air Saver";
                case "14":
                    return "UPS Next Day Air Early A.M.";
                case "54":
                    return "UPS Worldwide Express Plus";
                case "59":
                    return "UPS 2nd Day Air A.M.";
                case "65":
                    return "UPS Saver";
                case "82": //82-86, for Polish Domestic Shipments
                    return "UPS Today Standard";
                case "83":
                    return "UPS Today Dedicated Courier";
                case "85":
                    return "UPS Today Express";
                case "86":
                    return "UPS Today Express Saver";
                default:
                    return "Unknown";
            }
        }

        public static string GetUPSCode(string value)
        {
            string code = string.Empty;
            if (UPSServicesList().ContainsValue(value))
            {
                foreach (String key in UPSServicesList().Keys)
                {
                    if (UPSServicesList()[key].Equals(value))
                        return key;
                }
            }
            return code;
        }
        public static Dictionary<string, string> UPSServicesList()
        {
            Dictionary<string, string> servicesList = new Dictionary<string, string>();
            servicesList.Add("03", UPSServices.Ground.ToString());
            servicesList.Add("11", UPSServices.Standard.ToString());
            servicesList.Add("12", UPSServices.ThreeDaySelect.ToString());
            servicesList.Add("02", UPSServices.SecondDayAir.ToString());
            servicesList.Add("59", UPSServices.SecondDayAirAM.ToString());
            servicesList.Add("13", UPSServices.NextDayAir.ToString());
            servicesList.Add("01", UPSServices.Ground.ToString());
            servicesList.Add("14", UPSServices.NextDayAirEarlyAM.ToString());
            servicesList.Add("07", UPSServices.WorldwideExpress.ToString());
            servicesList.Add("54", UPSServices.WorldwideExpressPlus.ToString());
            servicesList.Add("08", UPSServices.WorldwideExpedited.ToString());
            servicesList.Add("65", UPSServices.ExpressSaver.ToString());
            return servicesList;
        }
        #endregion
    }


}
