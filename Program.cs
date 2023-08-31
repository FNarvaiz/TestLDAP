// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography.X509Certificates;
using System.DirectoryServices.Protocols;

using Newtonsoft.Json;
var ldapConf = new LdapConfinguration(){
    Server = "LDAP://ldap.cencosud.corp", // LDAP con mayuscula al inicio
    FileCer = "C:\\ldap.cencosud.corp.cer",
    BindDn = "Jose_Romero",
    BindPassword = "lalala1234"
};
var usuario = "jose.romero";
var password = "password";
Console.WriteLine("Hello, World!");
// Establecer el certificado de seguridad
X509Certificate2 cert = new X509Certificate2(ldapConf.FileCer);
// Agregar el certificado al almacen de certificados
var ldapConnection = new LdapConnection( new LdapDirectoryIdentifier(ldapConf.Server, ldapConf.PortNumber),
new System.Net.NetworkCredential(ldapConf.BindDn, ldapConf.BindPassword)
,AuthType.Basic);
ldapConnection.SessionOptions.SecureSocketLayer = true;
ldapConnection.SessionOptions.VerifyServerCertificate = new VerifyServerCertificateCallback((con, cer) => true);
ldapConnection.ClientCertificates.Add(cert);
// Establecer la versión de LDAP a 3 para no tener el error
ldapConnection.SessionOptions.ProtocolVersion = 3;

ldapConnection.Bind();
Console.WriteLine($"Conneccion establecida con el servidor: {ldapConf.Server}");

string search_base = "dc=cencosud,dc=corp";
string search_filter = $"(sAMAccountName={usuario})";
String[] attributes = new string[] { "cn", "userPrincipalName" };
var request = new SearchRequest(search_base, search_filter, SearchScope.Subtree, attributes );
var searchResponse = (SearchResponse) ldapConnection.SendRequest(request);
//convertir el objeto a json para poder ver el resultado de la respuesta
var json = JsonConvert.SerializeObject(searchResponse);
Console.WriteLine($"searchResponse:  {json}");
if (!searchResponse.Entries.Count.Equals(0)){
    var ubicacion_usuario = searchResponse.Entries[0].DistinguishedName;
    ldapConnection.Bind(new System.Net.NetworkCredential(ubicacion_usuario, password));
}
else{
    Console.WriteLine("Usuario no encontrado");
}
Console.WriteLine("TERMINADO");



public class LdapConfinguration{
        public string Server { get; set; } = "";
        public int PortNumber { get; set; } = 636; // El puerto predefinido de ldap es 636
        public string FileCer { get; set; } = "";
        public string BindDn { get; set; } = "";
        public string BindPassword { get; set; } = "";
    }