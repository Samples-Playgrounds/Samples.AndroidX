package com.example.mylibrary

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import androidx.activity.compose.setContent
import androidx.compose.animation.AnimatedVisibility
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.wrapContentWidth
import androidx.compose.material.Button
import androidx.compose.material.Icon
import androidx.compose.material.MaterialTheme
import androidx.compose.material.Text
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.AddChart
import androidx.compose.material.icons.filled.Menu
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.dimensionResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.tooling.preview.Preview

class ComposeActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            MaterialTheme {
                Greeting()
            }
        }
    }

    @Composable
    private fun Greeting() {
        var visible by remember { mutableStateOf(true) }

        Column {
            if (visible) {
                Text(
                    text = stringResource(R.string.greeting),
                    style = MaterialTheme.typography.h5,
                    modifier = Modifier
                        .fillMaxWidth()
                        .padding(horizontal = dimensionResource(R.dimen.margin_small))
                        .wrapContentWidth(Alignment.CenterHorizontally)
                )
            }
            Button(onClick = { visible = !visible }) {
                Row {
                    Icon(Icons.Filled.Menu, contentDescription = "Localized description")
                    Text("Toggle")
                    Icon(Icons.Filled.AddChart, contentDescription = "Localized description")
                }
            }
        }
    }

    @Composable
    @Preview
    private fun GreetingPreview() {
        Greeting()
    }
}