// Created by Crowbar 0.71

$modelname "Hyrule\inside_the_deku_tree\props\door.mdl"

$staticprop

$bodygroup "Body"
{
	studio "inside_the_deku_tree_props_doors.smd"
}


$surfaceprop "default"

$contents "solid"

$illumposition 0 0 58.517

$ambientboost

$mostlyopaque

$cdmaterials "Hyrule\inside_the_deku_tree\props\"
$cdmaterials ""

$texturegroup "skinfamilies"
{
	{ "ydan_tdoor_t_mo_0" }
	{ "ydan_tdoor_mode_0" }
}

$cbox 0 0 0 0 0 0

$bbox -10.64 -42.558 0 10.64 42.558 117.034

$definebone "static_prop" "" 0 0 0 0 0 0 0 0 0 0 0 0


$sequence "idle" {
	"door_anims\idle.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
}
