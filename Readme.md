<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/134075694/14.2.3%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E4610)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->
*Files to look at*:

* **[MultiEditASPxGridListEditor.cs](./CS/WebExample.Module.Web/MultiEditASPxGridListEditor.cs) (VB: [MultiEditASPxGridListEditor.vb](./VB/WebExample.Module.Web/MultiEditASPxGridListEditor.vb))**
<!-- default file list end -->
# OBSOLETE - How to implement multi-row editing in the ASP.NET ListView
<!-- run online -->
**[[Run Online]](https://codecentral.devexpress.com/e4610)**
<!-- run online end -->


<p><strong>NOTE:</strong> Starting with version 15.2, our ASPxGridListEditor supports the <a href="https://documentation.devexpress.com/AspNet/CustomDocument16443.aspx">Batch Edit Mode</a> out of the box. To enable it, set the IModelListView.AllowEdit property to True and the IModelListViewWeb.InlineEditMode property to Batch in the Model Editor, as described in the <a href="https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113249.aspx">List View Edit Modes</a> topic.</p>
<p><br><strong>For versions prior to 15.2:</strong></p>
<p>There are several examples on how to implement this functionality in the ASPxGridView without XAF. From my point of view, the <a href="https://www.devexpress.com/Support/Center/p/E2333">How to perform ASPxGridView instant updating using different editors in the DataItem template</a> example is the most appropriate for XAF, because:</p>
<p>- this approach can be easily implemented using runtime code</p>
<p>- we already use DataItem templates to show data in grid cells.</p>
<br>
<p>All functionality is implemented in a single controller - the <strong>MultiRowEditingController</strong>. It performs the following operations:</p>
<p>1. Creates an <strong>ASPxCallback</strong> control and adds it to a page. This control is used to send callbacks from client-side editors used in grid cells.</p>
<p>2. Replaces the default DataItemTemplate with a custom one (<strong>EditItemTemplate</strong>). The custom template is required to show editors for the user input in grid cells. This template is based on the DataItemTemplate class used in XAF by default. The only difference is that controls from this template are always in the Edit mode.</p>
<p>3. Assigns the client-side script that performs a callback when the value is changed to the editors added to grid cells via the custom DataItemTemplate. This is done in the editor's <strong>Init</strong> event handler, because at this moment, the NamingContainer that contains the key of the bound object is available.</p>
<p>4. Handles the callback sent from the client side and changes the value of a corresponding object's property.</p>
<br>
<p>I recommend that you review the following help topics for additional information:</p>
<p><a href="http://documentation.devexpress.com/#Xaf/CustomDocument3165"><u>Access Grid Control Properties</u></a><u><br> </u><u><a href="http://documentation.devexpress.com/#AspNet/DevExpressWebASPxGridViewASPxGridView_Templatestopic">ASPxGridView.Templates Property<br>How to use custom ASPxGridView template in a Web XAF application</a><br><br></u></p>
<p><strong>Important notes</strong><br>We have not tested this solution under all possible scenarios, so feel free to modify and test the code to better suit your needs. It this example and its current limitations do not meet your requirements, check out the <a href="https://www.devexpress.com/Support/Center/p/T213187">How to enable fast data entry in multiple rows for ListView on the Web (ASPxGridListEditor)?</a> thread for alternative solutions.</p>

<br/>


