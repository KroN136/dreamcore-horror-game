﻿using System;
using System.Collections.Generic;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class CollectedArtifact
{
    public Guid Id { get; set; }

    public Guid PlayerId { get; set; }

    public Guid ArtifactId { get; set; }

    public DateTime CollectionTimestamp { get; set; }

    public virtual Artifact? Artifact { get; set; }

    public virtual Player? Player { get; set; }
}
