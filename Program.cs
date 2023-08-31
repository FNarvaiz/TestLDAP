// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography.X509Certificates;
using System.DirectoryServices.Protocols;
using Newtonsoft.Json;
var ldapConifuration = new LdapConifuration(){
    Server = "ldap://ldap.cencosud.corp",
    PortNumber = "636",
    FileCer = "C:\\Users\\jose.romero\\Documents\\certificado\\ldap.cencosud.corp.cer",
    BindDn = "CN=Jose Romero,OU",
    BindPassword = "password"
};
var usuario = "jose.romero";
var password = "password";
Console.WriteLine("Hello, World!");
// Establecer el certificado de seguridad
X509Certificate2 cert = new X509Certificate2(ldapConifuration.FileCer);
// Agregar el certificado al almacen de certificados

var ldapConnection = new LdapConnection( new LdapDirectoryIdentifier(ldapConifuration.Server));
ldapConnection.SessionOptions.SecureSocketLayer = true;
ldapConnection.SessionOptions.VerifyServerCertificate = new VerifyServerCertificateCallback((con, cer) => true);
ldapConnection.ClientCertificates.Add(cert);
// Establecer la versión de LDAP a 3 para no tener el error
ldapConnection.SessionOptions.ProtocolVersion = 3;
ldapConnection.AuthType = AuthType.Basic; // Opcional: establecer el tipo de autenticación

ldapConnection.Bind(new System.Net.NetworkCredential(ldapConifuration.BindDn, ldapConifuration.BindPassword));
Console.WriteLine($"Conneccion establecida con el servidor: {ldapConifuration.Server}");

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



public class LdapConifuration{
        public string Server { get; set; } = "";
        public string PortNumber { get; set; } = "";
        public string FileCer { get; set; } = "";
        public string BindDn { get; set; } = "";
        public string BindPassword { get; set; } = "";
    }