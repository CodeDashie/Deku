// Created by Crowbar 0.71

$modelname "Hyrule\inside_the_deku_tree\inside_the_deku_tree_00.mdl"

$staticprop

$bodygroup "Body"
{
	studio "inside_the_deku_tree_00.smd"
}


$surfaceprop "default"

$contents "solid"

$illumposition -40.711 -208.401 123.519

$ambientboost

$mostlyopaque

$cdmaterials "Hyrule\inside_the_deku_tree\"
$cdmaterials ""

$cbox 0 0 0 0 0 0

$bbox -573.856 -652.458 -1095.1 990.658 571.036 1342.138

$definebone "static_prop" "" 0 0 0 0 0 0 0 0 0 0 0 0


$sequence "idle" {
	"inside_the_deku_tree_00_anims\idle.smd"
	fadein 0.2
	fadeout 0.2
	fps 30
}
