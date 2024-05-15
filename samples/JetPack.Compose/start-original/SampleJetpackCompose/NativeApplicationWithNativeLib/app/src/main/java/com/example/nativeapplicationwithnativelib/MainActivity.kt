package com.example.nativeapplicationwithnativelib

import android.app.Activity
import android.content.Intent
import android.os.Bundle
import com.example.mylibrary.ComposeActivity

class MainActivity : Activity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        val intent = Intent(this, ComposeActivity::class.java)
        startActivity(intent)
    }
}