using System;
using System.Collections.Generic;

namespace EER.Persistence.EFCore;

public partial class MigrationHistory
{
    public string MigrationId { get; set; } = null!;
}
