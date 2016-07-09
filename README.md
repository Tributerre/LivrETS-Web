# LivrETS-Web
Dépôt du module web du projet LivrETS de Tributerre. Ce projet est sous la licence GNU Public Licence version 3 ou GPLv3 et est donc libre d'être pris et redistribué sous la même licence. Tout le monde est invité à contribuer en produisant une fourche (fork) du projet et en créant une requête de tirage (pull request) après la modification. Pour plus de détails, voyez la section de contribution à la fin de ce fichier.

## Table des matières
* [Démarrage](#getting-started)
    * [Prérequis](#requirements)
    * [Manipulations](#manipulations)

<h2 id="getting-started">Démarrage</h2>
Le projet est relativement simple à démarrer. Il s'agit d'un projet ASP.NET version 4 sur le cadricel .NET version 4.5+ créé à l'aide du logiciel Visual Studio (VS) 2015 version communautaire. Le gestionnaire de dépendances du projet est nuget et le langage de programmation est C#.

<h3 id="requirements">Prérequis</h3>
* Minimum Visual Studio 2015 version communautaire;
* .NET version 4.5+;
* SQL Server 2014+ ou utiliser LocalDB dans VS directement.
 
<h3 id="manipulations">Manipulations</h3>
1. Clônez le dépôt localement sur votre machine.

    Astuce: Utilisez `git clone`...

2. Ouvrez le projet (LivrETS.sln) avec Visual Studio.

3. Copiez, collez et renommez les fichiers `Web.sample.config`, `Web.Debug.sample.config` et `Web.Release.sample.config` en `Web.config`, `Web.Debug.config` et `Web.Release.config` respectivement.

    Cette technique est utilisée pour ne pas mettre des clés de services par accident dans le dépôt.

4. Dans `Web.confg`, configurez votre environnement.

    Par exemple, vous voudrez peut être utiliser un vrai SQL Server au lieu du LocalDB de Visual Studio. Pour ce faire, trouvez et modifiez les lignes suivantes:
    ```xml
    <connectionStrings>
        <add name="DefaultConnection" connectionString="Data Source=(LocalDb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\aspnet-LivrETS-20160629111902.mdf;Initial Catalog=aspnet-LivrETS-20160629111902;Integrated Security=True"
          providerName="System.Data.SqlClient" />
        <!--<add name="DefaultConnection" connectionString="Data Source=YOUR_MACHINE_NAME;Initial Catalog=LivrETS;Integrated Security=SSPI;" providerName="System.Data.SqlClient" />-->
    </connectionStrings>
    ```
    L'étiquette (tag) commentez est celui d'un SQL Server configuré localement avec authentification par compte Windows. Le non-commenté est le LocalDB. Pour savoir quoi mettre dans l'attribut «connectionString», voyez [ce lien](https://connectionstrings.com/sql-server/). Il est possible que vous aurez à créer la base de données «livrets» dans SQL Server Management Studio.
    
    Autre chose à modifier si vous désirez que l'authentification fonctionne, il y a deux clés à insérer dans le fichier pour la redirection de l'authentification par Google.
    ```xml
    <appSettings>
        <add key="webpages:Version" value="3.0.0.0" />
        <add key="webpages:Enabled" value="false" />
        <add key="ClientValidationEnabled" value="true" />
        <add key="UnobtrusiveJavaScriptEnabled" value="true" />
        <add key="GoogleClientID" value="YOUR_GOOGLE_CLIENT_ID" />
        <add key="GoogleClientSecret" value="YOUR_GOOGLE_CLIENT_SECRET" />
    </appSettings>
    ```
    Les clés `GoogleClientID` et `GoogleClientSecret` sont utilisées par ASP.NET directement. Voyez l'exemple suivant tiré du projet:
    ```c#
    app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
    {
        ClientId = ConfigurationManager.AppSettings["GoogleClientID"],
        ClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"]
    });
    ```

5. Avec le projet configuré, il reste la base de données à mettre aux goûts du jour. Ouvrez la console de gestion de paquets nuget (Tools > Library Package Manager > Package Manager Console) et exécutez la commande `Update-Database`.
 
    Cette commande lance les migrations contenues dans le répertoire `Migrations`.

6. Exécutez le projet et voyez si le tout fonctionne. Si vous avez des problèmes, vous pouvez ouvrir un billet d'aide ou envoyer un courriel à l'un des contributeurs enregistrés du projet.
