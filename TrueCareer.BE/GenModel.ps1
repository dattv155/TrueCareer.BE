dotnet ef dbcontext scaffold  "data source=222.252.27.58,2022;initial catalog=TrueCareer;persist security info=True;user id=sa;password=123@123a;multipleactiveresultsets=True;" Microsoft.EntityFrameworkCore.SqlServer -c DataContext  -o Models -f --no-build --use-database-names --json
$content = Get-Content -Path 'Models\DataContext.cs' -Encoding UTF8
$content = $content -replace "using System;", "using System;using Thinktecture;"
$content = $content -replace "modelBuilder.Entity<ActionDAO>", "modelBuilder.ConfigureTempTable<long>();modelBuilder.ConfigureTempTable<Guid>();modelBuilder.Entity<ActionDAO>"
$content | Set-Content -Path "Models\DataContext.cs"  -Encoding UTF8