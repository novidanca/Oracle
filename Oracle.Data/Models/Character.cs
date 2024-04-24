﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oracle.Data.Models;

public class Character
{
    [Key] public int Id { get; set; }
    public required string Name { get; set; }
    public Player? Player { get; set; }
    public ICollection<Project> Projects { get; } = [];
    public ICollection<AdventureCharacter> AdventureCharacters { get; } = [];
    public ICollection<Activity> Activities { get; } = [];
}